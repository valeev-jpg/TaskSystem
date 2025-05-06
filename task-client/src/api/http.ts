import axios from "axios";

/* ────────── базовая настройка ────────── */
const api = axios.create({
  baseURL: process.env.REACT_APP_API_URL ?? "http://localhost:5137/api",
  withCredentials: true,              // ← браузер будет слать cookie refresh
});

/* ────────── helper: fingerprint ─────── */
function getDeviceId(): string {
  const key = "deviceId";
  let id = localStorage.getItem(key);
  if (!id) {
    id = crypto.randomUUID();
    localStorage.setItem(key, id);
  }
  return id;
}

/* ────────── request‑интерсептор ─────── */
api.interceptors.request.use((cfg) => {
  const token = localStorage.getItem("accessToken");
  if (token) cfg.headers!.Authorization = `Bearer ${token}`;
  cfg.headers!["X-Device-Id"] = getDeviceId();
  return cfg;
});

/* ────────── response‑интерсептор (refresh) ─────── */
let refreshPromise: Promise<string> | null = null;

api.interceptors.response.use(
  (r) => r,
  async (err) => {
    const orig = err.config;
    if (err.response?.status === 401 && !orig._retry) {
      orig._retry = true;
      try {
        if (!refreshPromise) refreshPromise = refreshToken();
        const newAccess = await refreshPromise;
        refreshPromise = null;

        localStorage.setItem("accessToken", newAccess);
        orig.headers.Authorization = `Bearer ${newAccess}`;
        return api(orig);
      } catch {
        localStorage.clear();
        window.location.href = "/login";
      }
    }
    return Promise.reject(err);
  }
);

/* ────────── POST /auth/refresh (cookie) ─────── */
async function refreshToken(): Promise<string> {
  const { data } = await axios.post<{ accessToken: string }>(
    "/auth/refresh",
    null,                               // тело не нужно — cookie уже внутри
    { baseURL: api.defaults.baseURL, withCredentials: true }
  );
  return data.accessToken;
}

export default api;
