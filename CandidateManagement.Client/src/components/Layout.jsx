import { Outlet } from "react-router-dom";
import ActivityFeed from "./ActivityFeed";
import Navbar from "./Navbar";

function Layout() {
  return (
    <div className="app">
      <Navbar />

      <main className="main-content">
        <div className="main-content__body">
          <Outlet />
        </div>

        <ActivityFeed />
      </main>
    </div>
  );
}

export default Layout;
