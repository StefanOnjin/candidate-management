import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { deleteCandidate, getCandidates, searchCandidates } from "../api/candidatesApi";
import { getSkills } from "../api/skillsApi";
import SearchBar from "../components/SearchBar";

function CandidatesPage() {
  const [candidates, setCandidates] = useState([]);
  const [openCandidateId, setOpenCandidateId] = useState(null);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize] = useState(6);
  const [totalCount, setTotalCount] = useState(0);
  const [isSearchMode, setIsSearchMode] = useState(false);
  const [skills, setSkills] = useState([]);
  const [selectedSkillId, setSelectedSkillId] = useState("");

  useEffect(() => {
    loadSkills();
  }, []);

  useEffect(() => {
    if (isSearchMode) return;
    loadCandidates(currentPage);
  }, [currentPage, isSearchMode]);

  async function loadCandidates(page) {
    try {
      const response = await getCandidates(page, pageSize);
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

  async function handleSearchByName(fullName) {
    const skillIds = selectedSkillId ? [Number(selectedSkillId)] : [];

    if (!fullName && skillIds.length === 0) {
      setIsSearchMode(false);
      setCurrentPage(1);
      await loadCandidates(1);
      return;
    }

    try {
      const response = await searchCandidates({ fullName, skillIds });
      setCandidates(response.data);
      setTotalCount(response.data.length);
      setOpenCandidateId(null);
      setIsSearchMode(true);
    } catch (error) {
      alert(error.response?.data || "Failed to search candidates.");
    }
  }

  async function handleClearSearch() {
    setSelectedSkillId("");
    setIsSearchMode(false);
    setCurrentPage(1);
    setOpenCandidateId(null);
    await loadCandidates(1);
  }

  async function handleDeleteCandidate(id) {
    const confirmed = window.confirm(
      "Are you sure that you want to delete this candidate?"
    );

    if (!confirmed) return;

    try {
      await deleteCandidate(id);
      await loadCandidates(currentPage);
    } catch (error) {
      alert(error.response?.data || "Failed to delete candidate.");
    }
  }

  function toggleCandidateDetails(id) {
    setOpenCandidateId((currentId) => (currentId === id ? null : id));
  }

  const totalPages = Math.max(1, Math.ceil(totalCount / pageSize));

  function goToPage(page) {
    if (page < 1 || page > totalPages) return;
    setOpenCandidateId(null);
    setCurrentPage(page);
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
            onChange={(event) => setSelectedSkillId(event.target.value)}
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

            {!isSearchMode && (
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
            )}
          </div>
        )}
      </div>
    </section>
  );
}

export default CandidatesPage;
