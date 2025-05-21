import { useAuth } from "../../auth/AuthContext";
import { useEffect, useState } from "react";
import api from "../../api/http";
import {
  AppBar,
  Toolbar,
  Typography,
  IconButton,
  Button,
  Stack,
  Checkbox,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogContentText,
  DialogActions,
  CssBaseline,
  ThemeProvider,
  createTheme,
  Container,
} from "@mui/material";
import AddIcon from "@mui/icons-material/Add";
import LogoutIcon from "@mui/icons-material/Logout";
import AccountCircleIcon from "@mui/icons-material/AccountCircle";
import TicketCard from "./TicketCard";
import TicketModal from "./TicketModal";
import { TaskTicket } from "./types";

export default function TasksPage() {
  const { logout } = useAuth();
  const role = sessionStorage.getItem("userRole") ?? "";
  const isManager = role === "manager";

  const [tasks, setTasks] = useState<TaskTicket[]>([]);
  const [open, setOpen] = useState(false);
  const [edit, setEdit] = useState<TaskTicket | null>(null);
  const [sel, setSel] = useState<Set<string>>(new Set());
  const [showArchived, setShowArchived] = useState(false);
  const [profileOpen, setProfileOpen] = useState(false);

  // Новое состояние для темы:
  const [darkMode, setDarkMode] = useState(false);

  // Динамическая тема:
  const theme = createTheme({
    palette: {
      mode: darkMode ? "dark" : "light",
    },
  });

  const load = () =>
    api
      .get<TaskTicket[]>("/TaskTicket/GetAll")
      .then((r) => setTasks(r.data))
      .catch(console.error);

  useEffect(() => {
    load();
  }, []);

  const saveTask = async (patch: Partial<TaskTicket>) => {
    if (edit) {
      await api.put("/TaskTicket/Update", { ...edit, ...patch });
    } else {
      await api.post("/TaskTicket/Create", patch);
    }
    setEdit(null);
    setOpen(false);
    load();
  };

  const bulkArchive = () =>
    Promise.all(
      Array.from(sel).map((id) => {
        const t = tasks.find((x) => x.id === id)!;
        return api.put("/TaskTicket/Update", { ...t, archived: true });
      })
    ).then(() => {
      setSel(new Set());
      load();
    });

  const bulkUnarchive = () =>
    Promise.all(
      Array.from(sel).map((id) => {
        const t = tasks.find((x) => x.id === id)!;
        return api.put("/TaskTicket/Update", { ...t, archived: false });
      })
    ).then(() => {
      setSel(new Set());
      load();
    });

  const bulkComplete = () =>
    Promise.all(
      Array.from(sel).map((id) => {
        const t = tasks.find((x) => x.id === id)!;
        return api.put("/TaskTicket/Update", { ...t, status: "Completed" });
      })
    ).then(() => {
      setSel(new Set());
      load();
    });

  const bulkDelete = () =>
    Promise.all(
      Array.from(sel).map((id) => api.delete(`/TaskTicket/Delete?id=${id}`))
    ).then(() => {
      setSel(new Set());
      load();
    });

  const filtered = tasks.filter((t) => t.archived === showArchived);
  const activeCount = tasks.filter((t) => !t.archived).length;
  const archivedCount = tasks.filter((t) => t.archived).length;

  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <AppBar position="static">
        <Toolbar>
          <Typography sx={{ flexGrow: 1 }}>
            {showArchived ? "Archived Tickets" : "Active Tickets"}
          </Typography>

          {/* Кнопка переключения темы */}
          <Button
            color="inherit"
            onClick={() => setDarkMode(!darkMode)}
            sx={{ mr: 2 }}
          >
            {darkMode ? "Светлая тема" : "Тёмная тема"}
          </Button>

          <Button
            color="inherit"
            onClick={() => {
              setSel(new Set());
              setShowArchived(!showArchived);
            }}
          >
            {showArchived ? "View Active" : "View Archive"}
          </Button>
          <IconButton color="inherit" onClick={() => setProfileOpen(true)}>
            <AccountCircleIcon />
          </IconButton>
          {isManager && !showArchived && (
            <IconButton color="inherit" onClick={() => setOpen(true)}>
              <AddIcon />
            </IconButton>
          )}
          <IconButton color="inherit" onClick={logout}>
            <LogoutIcon />
          </IconButton>
        </Toolbar>
      </AppBar>

      {isManager && sel.size > 0 && (
        <Stack direction="row" spacing={1} sx={{ m: 2 }}>
          {!showArchived ? (
            <>
              <Button onClick={bulkComplete}>Complete ({sel.size})</Button>
              <Button onClick={bulkArchive}>Archive ({sel.size})</Button>
            </>
          ) : (
            <Button onClick={bulkUnarchive}>Unarchive ({sel.size})</Button>
          )}
          <Button color="error" onClick={bulkDelete}>
            Delete ({sel.size})
          </Button>
          <Checkbox
            checked={sel.size === filtered.length}
            indeterminate={sel.size > 0 && sel.size < filtered.length}
            onChange={(_, v) =>
              setSel(v ? new Set(filtered.map((t) => t.id)) : new Set())
            }
          />
          Select all
        </Stack>
      )}

      <Container maxWidth="md">
        {filtered.map((t) => (
          <TicketCard
            key={t.id}
            t={t}
            selectable={isManager}
            isManager={isManager}
            onSelect={(id, c) => {
              const s = new Set(sel);
              c ? s.add(id) : s.delete(id);
              setSel(s);
            }}
            onAction={(act) => {
              if (act === "edit") {
                setEdit(t);
                setOpen(true);
              }
              if (act === "archive")
                api.put("/TaskTicket/Update", { ...t, archived: true }).then(load);
              if (act === "unarchive")
                api.put("/TaskTicket/Update", { ...t, archived: false }).then(load);
              if (act === "delete") api.delete(`/TaskTicket/Delete?id=${t.id}`).then(load);
              if (act === "done")
                api
                  .put("/TaskTicket/Update", {
                    ...t,
                    status: "Completed",
                  })
                  .then(load);
              if (act === "start")
                api
                  .put("/TaskTicket/Update", {
                    ...t,
                    status: "InProgress",
                  })
                  .then(load);
            }}
          />
        ))}
      </Container>

      <TicketModal
        open={open}
        onClose={() => {
          setOpen(false);
          setEdit(null);
        }}
        initial={edit}
        onSave={saveTask}
      />

      <Dialog open={profileOpen} onClose={() => setProfileOpen(false)}>
        <DialogTitle>Личный кабинет</DialogTitle>
        <DialogContent>
          <DialogContentText>
            <strong>Роль:</strong> {role}
            <br />
            <strong>Активных тикетов:</strong> {activeCount}
            <br />
            <strong>В архиве:</strong> {archivedCount}
          </DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setProfileOpen(false)}>Закрыть</Button>
        </DialogActions>
      </Dialog>
    </ThemeProvider>
  );
}
