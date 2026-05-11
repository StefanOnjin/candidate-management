import { useEffect, useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";

import {
  createCandidate,
  getCandidateById,
  updateCandidate,
} from "../api/candidatesApi";

import { getSkills } from "../api/skillsApi";

function CandidateFormPage() {
  const { id } = useParams();
  const navigate = useNavigate();

  const isEditMode = Boolean(id);

  const [skills, setSkills] = useState([]);

  const [formData, setFormData] = useState({
    fullName: "",
    dateOfBirth: "",
    contactNumber: "",
    emailAddress: "",
    skillIds: [],
  });

  useEffect(() => {
    loadSkills();

    if (isEditMode) {
      loadCandidate();
    }
  }, [id]);

  async function loadSkills() {
    try {
      const response = await getSkills();
      setSkills(response.data);
    } catch (error) {
      console.error("Failed to load skills.", error);
    }
  }

  async function loadCandidate() {
    try {
      const response = await getCandidateById(id);
      const candidate = response.data;

      setFormData({
        fullName: candidate.fullName,
        dateOfBirth: candidate.dateOfBirth,
        contactNumber: candidate.contactNumber,
        emailAddress: candidate.emailAddress,
        skillIds: candidate.skills.map((skill) => skill.id),
      });
    } catch (error) {
      console.error("Failed to load candidate.", error);
      alert("Failed to load candidate.");
    }
  }

  function handleInputChange(event) {
    const { name, value } = event.target;

    setFormData({
      ...formData,
      [name]: value,
    });
  }

  function handleSkillCheckboxChange(skillId) {
    const isSelected = formData.skillIds.includes(skillId);

    let updatedSkillIds;

    if (isSelected) {
      updatedSkillIds = formData.skillIds.filter((id) => id !== skillId);
    } else {
      updatedSkillIds = [...formData.skillIds, skillId];
    }

    setFormData({
      ...formData,
      skillIds: updatedSkillIds,
    });
  }
  async function handleSubmit(event) {
    event.preventDefault();

    try {
      if (isEditMode) {
        await updateCandidate(id, formData);
      } else {
        await createCandidate(formData);
      }

      navigate("/candidates");
    } catch (error) {
      alert(error.response?.data || "Failed to save candidate.");
    }
  }

  return (
    <section className="page">
      <div className="page-header">
        <div>
          <h1>{isEditMode ? "Edit Candidate" : "Add Candidate"}</h1>
          <p>
            {isEditMode
              ? "Update information and/or skills."
              : "Create new job candidate."}
          </p>
        </div>

        <Link className="button-link" to="/candidates">
          Back
        </Link>
      </div>

      <div className="card">
        <form className="candidate-form" onSubmit={handleSubmit}>
          <div className="form-group">
            <label>Full name</label>
            <input
              type="text"
              name="fullName"
              value={formData.fullName}
              onChange={handleInputChange}
              required
            />
          </div>

          <div className="form-group">
            <label>Date of birth</label>
            <input
              type="date"
              name="dateOfBirth"
              value={formData.dateOfBirth}
              onChange={handleInputChange}
              required
            />
          </div>

          <div className="form-group">
            <label>Contact number</label>
            <input
              type="text"
              name="contactNumber"
              value={formData.contactNumber}
              onChange={handleInputChange}
              required
            />
          </div>

          <div className="form-group">
            <label>Email address</label>
            <input
              type="email"
              name="emailAddress"
              value={formData.emailAddress}
              onChange={handleInputChange}
              required
            />
          </div>

          <div className="form-group">
  <label>Skills</label>

  <div className="skills-checkbox-list">
      {skills.map((skill) => (
        <label key={skill.id} className="skill-checkbox">
          <input
            type="checkbox"
            checked={formData.skillIds.includes(skill.id)}
            onChange={() => handleSkillCheckboxChange(skill.id)}
          />
          {skill.skillName}
        </label>
      ))}
    </div>
  </div>

          <div className="form-actions">
            <button type="submit">
              {isEditMode ? "Save Changes" : "Create Candidate"}
            </button>

            <Link className="button-secondary" to="/candidates">
              Cancel
            </Link>
          </div>
        </form>
      </div>
    </section>
  );
}

export default CandidateFormPage;