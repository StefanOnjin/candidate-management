import { useEffect, useState } from "react";
import { createSkill, deleteSkill, getSkills } from "../api/skillsApi";
import SearchBar from "../components/SearchBar";

function SkillsPage() {
  const [skills, setSkills] = useState([]);
  const [skillName, setSkillName] = useState("");
  const [searchTerm, setSearchTerm] = useState("");

  useEffect(() => {
    loadSkills();
  }, []);

  async function loadSkills() {
    try {
      const response = await getSkills();
      setSkills(response.data);
    } catch (error) {
      console.error("Failed to load skills.", error);
    }
  }

  async function handleCreateSkill(event) {
    event.preventDefault();

    if (!skillName.trim()) {
      alert("Skill name is required.");
      return;
    }

    try {
      await createSkill({
        skillName: skillName.trim(),
      });

      setSkillName("");
      await loadSkills();
    } catch (error) {
      alert(error.response?.data || "Failed to create skill.");
    }
  }

  async function handleDeleteSkill(id) {
    const confirmed = window.confirm(
      "Are you sure that you want to delete this skill?"
    );

    if (!confirmed) return;

    try {
      await deleteSkill(id);
      await loadSkills();
    } catch (error) {
      alert(error.response?.data || "Failed to delete skill.");
    }
  }

  function handleSearch(term) {
    setSearchTerm(term.toLowerCase());
  }

  function handleClearSearch() {
    setSearchTerm("");
  }

  const filteredSkills = skills.filter((skill) =>
    skill.skillName.toLowerCase().includes(searchTerm)
  );

  return (
    <section className="page">
      <div className="page-header">
        <div>
          <h1>Skills</h1>
          <p>Manage skills that can be assigned to job candidates.</p>
        </div>
      </div>

      <div className="card">
        <h2>Add new skill</h2>

        <form className="form-row" onSubmit={handleCreateSkill}>
          <input
            type="text"
            placeholder="Example: C# programming"
            value={skillName}
            onChange={(event) => setSkillName(event.target.value)}
          />

          <button type="submit">Add Skill</button>
        </form>
      </div>

      <div className="card">
        <h2>Skill list</h2>
        <SearchBar
          label="Search skill by name"
          placeholder="Example: C# programming"
          onSearch={handleSearch}
          onClear={handleClearSearch}
        />

        {filteredSkills.length === 0 ? (
          <p>No skills found.</p>
        ) : (
          <table className="table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Skill name</th>
                <th className="table-actions">Actions</th>
              </tr>
            </thead>

            <tbody>
              {filteredSkills.map((skill) => (
                <tr key={skill.id}>
                  <td>{skill.id}</td>
                  <td>{skill.skillName}</td>
                  <td className="table-actions">
                    <button
                      className="button-danger"
                      onClick={() => handleDeleteSkill(skill.id)}
                    >
                      Delete
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
    </section>
  );
}

export default SkillsPage;
