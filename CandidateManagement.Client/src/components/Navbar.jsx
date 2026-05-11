import { NavLink } from "react-router-dom";

function Navbar() {
  return (
    <nav className="navbar">
      <div className="navbar__brand">Candidate Management</div>

      <div className="navbar__links">
        <NavLink to="/candidates">Candidates</NavLink>
        <NavLink to="/skills">Skills</NavLink>
      </div>
    </nav>
  );
}

export default Navbar;