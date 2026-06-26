import { useEffect, useState } from "react";
import { useSearchParams } from "react-router-dom";
import { getActivities } from "../api/activitiesApi";
import SearchBar from "../components/SearchBar";

const PAGE_SIZE = 8;

function ActivitiesPage() {
  const [searchParams, setSearchParams] = useSearchParams();
  const [activities, setActivities] = useState([]);
  const [totalCount, setTotalCount] = useState(0);
  const [errorMessage, setErrorMessage] = useState("");

  const currentPage = Math.max(1, Number(searchParams.get("page")) || 1);
  const searchTerm = searchParams.get("search") ?? "";
  const selectedEventType = searchParams.get("eventType") ?? "";
  const selectedEntityType = searchParams.get("entityType") ?? "";

  useEffect(() => {
    loadActivities();
  }, [searchParams]);

  async function loadActivities() {
    setErrorMessage("");

    const params = {
      page: currentPage,
      pageSize: PAGE_SIZE,
    };

    if (searchTerm.trim()) {
      params.search = searchTerm.trim();
    }

    if (selectedEventType) {
      params.eventType = selectedEventType;
    }

    if (selectedEntityType) {
      params.entityType = selectedEntityType;
    }

    try {
      const response = await getActivities(params);
      setActivities(response.data.items);
      setTotalCount(response.data.totalCount);
    } catch (error) {
      console.error("Failed to load activities.", error);
      setActivities([]);
      setTotalCount(0);
      setErrorMessage("Failed to load activity history. Please try again.");
    }
  }

  function updateListParams(nextValues) {
    const nextParams = new URLSearchParams(searchParams);

    Object.entries(nextValues).forEach(([key, value]) => {
      if (value === "" || value === null || value === undefined) {
        nextParams.delete(key);
        return;
      }

      nextParams.set(key, String(value));
    });

    setSearchParams(nextParams);
  }

  function handleSearch(term) {
    updateListParams({
      search: term,
      page: 1,
    });
  }

  function handleClearSearch() {
    updateListParams({
      search: "",
      eventType: "",
      entityType: "",
      page: 1,
    });
  }

  function handleEventTypeChange(event) {
    updateListParams({
      eventType: event.target.value,
      page: 1,
    });
  }

  function handleEntityTypeChange(event) {
    updateListParams({
      entityType: event.target.value,
      page: 1,
    });
  }

  const totalPages = Math.max(1, Math.ceil(totalCount / PAGE_SIZE));

  function goToPage(page) {
    if (page < 1 || page > totalPages) return;

    updateListParams({
      page,
    });
  }

  return (
    <section className="page">
      <div className="page-header">
        <div>
          <h1>Activities</h1>
          <p>Review the persisted history of system activity events.</p>
        </div>
      </div>

      <div className="card">
        <h2>Activity history</h2>

        <div className="activities-filter-grid">
          <div className="activities-filter-field">
            <label htmlFor="eventTypeFilter">Filter by event type</label>
            <select
              id="eventTypeFilter"
              value={selectedEventType}
              onChange={handleEventTypeChange}
            >
              <option value="">All event types</option>
              <option value="candidate.created">Candidate created</option>
              <option value="candidate.updated">Candidate updated</option>
              <option value="candidate.deleted">Candidate deleted</option>
              <option value="candidate.skill_removed">Candidate skill removed</option>
              <option value="skill.created">Skill created</option>
              <option value="skill.updated">Skill updated</option>
              <option value="skill.deleted">Skill deleted</option>
            </select>
          </div>

          <div className="activities-filter-field">
            <label htmlFor="entityTypeFilter">Filter by entity type</label>
            <select
              id="entityTypeFilter"
              value={selectedEntityType}
              onChange={handleEntityTypeChange}
            >
              <option value="">All entity types</option>
              <option value="candidate">Candidate</option>
              <option value="skill">Skill</option>
            </select>
          </div>
        </div>

        <SearchBar
          label="Search activity message or entity name"
          placeholder="Example: Marko Markovic"
          initialValue={searchTerm}
          onSearch={handleSearch}
          onClear={handleClearSearch}
        />

        {activities.length === 0 ? (
          <p>No activities found.</p>
        ) : (
          <div>
            <table className="table">
              <thead>
                <tr>
                  <th>Time</th>
                  <th>Event</th>
                  <th>Entity</th>
                  <th>Entity name</th>
                  <th>Message</th>
                </tr>
              </thead>

              <tbody>
                {activities.map((activity) => (
                  <tr key={activity.eventId}>
                    <td className="activities-time-cell">
                      {formatActivityDateTime(activity.occurredAtUtc)}
                    </td>
                    <td>{formatLabel(activity.eventType)}</td>
                    <td>{formatLabel(activity.entityType)}</td>
                    <td>{activity.entityName}</td>
                    <td>{activity.message}</td>
                  </tr>
                ))}
              </tbody>
            </table>

            <div className="pagination">
              <button
                type="button"
                className="button-small pagination-button"
                onClick={() => goToPage(currentPage - 1)}
                disabled={currentPage === 1}
              >
                Previous
              </button>

              {Array.from({ length: totalPages }, (_, index) => index + 1).map(
                (page) => (
                  <button
                    type="button"
                    key={page}
                    className={`button-small pagination-button ${page === currentPage ? "is-active" : ""}`}
                    onClick={() => goToPage(page)}
                  >
                    {page}
                  </button>
                )
              )}

              <button
                type="button"
                className="button-small pagination-button"
                onClick={() => goToPage(currentPage + 1)}
                disabled={currentPage === totalPages}
              >
                Next
              </button>
            </div>
          </div>
        )}
      </div>

      {errorMessage && (
        <div className="status-banner status-banner--error status-banner--floating" role="alert">
          <strong className="status-banner__title">Something went wrong</strong>
          <p className="status-banner__message">{errorMessage}</p>
        </div>
      )}
    </section>
  );
}

function formatLabel(value) {
  if (!value) return "";

  return value
    .split(/[._]/)
    .filter(Boolean)
    .map((segment) => segment.charAt(0).toUpperCase() + segment.slice(1))
    .join(" ");
}

function formatActivityDateTime(value) {
  if (!value) return "";

  return new Date(value).toLocaleString([], {
    year: "numeric",
    month: "short",
    day: "2-digit",
    hour: "2-digit",
    minute: "2-digit",
  });
}

export default ActivitiesPage;
