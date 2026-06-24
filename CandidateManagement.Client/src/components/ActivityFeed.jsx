import { useEffect, useRef, useState } from "react";
import { createActivityHubConnection } from "../api/activityHubClient";

const MAX_ACTIVITY_ITEMS = 20;

function ActivityFeed() {
  const connectionRef = useRef(null);
  const [activities, setActivities] = useState([]);
  const [connectionStatus, setConnectionStatus] = useState("connecting");

  useEffect(() => {
    const connection = createActivityHubConnection();
    connectionRef.current = connection;

    connection.on("ActivityReceived", (activityEvent) => {
      setActivities((currentActivities) => [
        activityEvent,
        ...currentActivities,
      ].slice(0, MAX_ACTIVITY_ITEMS));
    });

    connection.onreconnecting(() => {
      setConnectionStatus("reconnecting");
    });

    connection.onreconnected(() => {
      setConnectionStatus("connected");
    });

    connection.onclose(() => {
      setConnectionStatus("disconnected");
    });

    connection
      .start()
      .then(() => setConnectionStatus("connected"))
      .catch((error) => {
        console.error("Failed to connect to activity hub.", error);
        setConnectionStatus("disconnected");
      });

    return () => {
      connection.stop();
    };
  }, []);

  return (
    <aside className="activity-feed">
      <div className="activity-feed__header">
        <div>
          <h2>Live Activity</h2>
          <span className={`activity-feed__status activity-feed__status--${connectionStatus}`}>
            {connectionStatus}
          </span>
        </div>
      </div>

      {activities.length === 0 ? (
        <p className="activity-feed__empty">No activity yet.</p>
      ) : (
        <ul className="activity-feed__list">
          {activities.map((activity) => (
            <li key={activity.eventId} className="activity-feed__item">
              <p>{activity.message}</p>
              <time>{formatActivityTime(activity.occurredAtUtc)}</time>
            </li>
          ))}
        </ul>
      )}
    </aside>
  );
}

function formatActivityTime(value) {
  if (!value) return "";

  return new Date(value).toLocaleTimeString([], {
    hour: "2-digit",
    minute: "2-digit",
    second: "2-digit",
  });
}

export default ActivityFeed;
