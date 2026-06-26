import { Navigate, Route, Routes } from "react-router-dom";

import Layout from "./components/Layout";
import ActivitiesPage from "./pages/ActivitiesPage";
import CandidatePage from "./pages/CandidatePage";
import CandidateFormPage from "./pages/CandidateFormPage";
import SkillPage from "./pages/SkillPage";

function App() {
  return (
    <Routes>
      <Route path="/" element={<Layout />}>
        <Route index element={<Navigate to="/candidates" replace />} />

        <Route path="candidates" element={<CandidatePage />} />
        <Route path="candidates/create" element={<CandidateFormPage />} />
        <Route path="candidates/edit/:id" element={<CandidateFormPage />} />

        <Route path="skills" element={<SkillPage />} />
        <Route path="activities" element={<ActivitiesPage />} />
      </Route>
    </Routes>
  );
}

export default App;
