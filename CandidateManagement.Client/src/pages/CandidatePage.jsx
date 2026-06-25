import { useEffect, useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import { deleteCandidate, getCandidates } from "../api/candidatesApi";
import { getSkills } from "../api/skillsApi";
import SearchBar from "../components/SearchBar";

const PAGE_SIZE = 6;

function CandidatesPage() {
  const [searchParams, setSearchParams] = useSearchParams();
  const [candidates, setCandidates] = useState([]);
  const [openCandidateId, setOpenCandidateId] = useState(null);
  const [totalCount, setTotalCount] = useState(0);
  const [skills, setSkills] = useState([]);

  const currentPage = Math.max(1, Number(searchParams.get("page")) || 1);
  const searchTerm = searchParams.get("search") ?? "";
  const selectedSkillId = searchParams.get("skillId") ?? "";

  useEffect(() => {
    loadSkills();
  }, []);

  useEffect(() => {
    setOpenCandidateId(null);
    loadCandidates();
  }, [searchParams]);

  async function loadCandidates() {
    const params = {
      page: currentPage,
      pageSize: PAGE_SIZE,
    };

    if (searchTerm.trim()) {
      params.search = searchTerm.trim();
    }

    if (selectedSkillId) {
      params.skillId = Number(selectedSkillId);
    }

    try {
      const response = await getCandidates(params);
      setCandidates(response.data.items);
      setTotalCount(response.data.totalCount);
    } catch (error) {
      console.error("Failed to load candidates.", error);
    }
  }

  async function loadSkills() {
    try {
      const response = await getSkills();
      setSkills(response.data);
    } catch (error) {
      console.error("Failed to load skills.", error);
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

  function handleSearchByName(fullName) {
    updateListParams({
      search: fullName,
      page: 1,
    });
  }

  function handleClearSearch() {
    updateListParams({
      search: "",
      skillId: "",
      page: 1,
    });
  }

  function handleSkillFilterChange(event) {
    updateListParams({
      skillId: event.target.value,
      page: 1,
    });
  }

  async function handleDeleteCandidate(id) {
    const confirmed = window.confirm(
      "Are you sure that you want to delete this candidate?"
    );

    if (!confirmed) return;

    try {
      await deleteCandidate(id);
      await loadCandidates();
    } catch (error) {
      alert(error.response?.data || "Failed to delete candidate.");
    }
  }

  function toggleCandidateDetails(id) {
    setOpenCandidateId((currentId) => (currentId === id ? null : id));
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
          <h1>Candidates</h1>
          <p>Manage job candidates and their skills.</p>
        </div>

        <Link className="button-link" to="/candidates/create">
          Add Candidate
        </Link>
      </div>

      <div className="card">
        <h2>Candidate list</h2>
        <div className="candidate-filter-row">
          <label htmlFor="skillFilter">Filter by skill</label>
          <select
            id="skillFilter"
            value={selectedSkillId}
            onChange={handleSkillFilterChange}
          >
            <option value="">All skills</option>
            {skills.map((skill) => (
              <option key={skill.id} value={skill.id}>
                {skill.skillName}
              </option>
            ))}
          </select>
        </div>

        <SearchBar
          label="Search candidate by name (optional)"
          placeholder="Example: Djordje Djordjevic"
          initialValue={searchTerm}
          onSearch={handleSearchByName}
          onClear={handleClearSearch}
        />

        {candidates.length === 0 ? (
          <p>No candidates found.</p>
        ) : (
          <div>
            <div className="candidate-accordion">
              {candidates.map((candidate) => {
                const isOpen = openCandidateId === candidate.id;

                return (
                  <article key={candidate.id} className="candidate-item">
                    <button
                      type="button"
                      className="candidate-item__header"
                      onClick={() => toggleCandidateDetails(candidate.id)}
                    >
                      <span className="candidate-item__name">{candidate.fullName}</span>
                      <span className="candidate-item__toggle">
                        {isOpen ? "Hide details" : "Show details"}
                      </span>
                    </button>

                    {isOpen && (
                      <div className="candidate-item__body">
                        <p>
                          <strong>Date of birth:</strong> {candidate.dateOfBirth}
                        </p>
                        <p>
                          <strong>Contact:</strong> {candidate.contactNumber}
                        </p>
                        <p>
                          <strong>Email:</strong> {candidate.emailAddress}
                        </p>
                        <p>
                          <strong>Skills:</strong>{" "}
                          {candidate.skills.map((skill) => skill.skillName).join(", ")}
                        </p>

                        <div className="candidate-item__actions">
                          <Link
                            className="button-small"
                            to={`/candidates/edit/${candidate.id}`}
                          >
                            Edit
                          </Link>

                          <button
                            className="button-danger button-small"
                            onClick={() => handleDeleteCandidate(candidate.id)}
                          >
                            Delete
                          </button>
                        </div>
                      </div>
                    )}
                  </article>
                );
              })}
            </div>

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
    </section>
  );
}

export default CandidatesPage;
