import { useAuth } from "../../auth/AuthContext";
import { useEffect, useState } from "react";
import api from "../../api/http";
import {
  AppBar, Toolbar, Typography, IconButton, Button,
  Stack, Checkbox
} from "@mui/material";
import AddIcon from "@mui/icons-material/Add";
import LogoutIcon from "@mui/icons-material/Logout";
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

  const load = () =>
    api.get<TaskTicket[]>("/TaskTicket/GetAll")
       .then(r => setTasks(r.data))
       .catch(console.error);   // желательно перехватить ошибку
  
  /* ───── effect ───── */
  useEffect(() => { load(); }, []);

  const saveTask = async (patch: Partial<TaskTicket>) => {
    if (edit) {
      console.log(edit)
      console.log(patch)
      await api.put("/TaskTicket/Update", { ...edit, ...patch });
    }
    else {
      console.log(patch)
      await api.post("/TaskTicket/Create", patch);
    }
    setEdit(null); setOpen(false); load();
  };

  const bulkArchive = () =>
    Promise.all(Array.from(sel).map(id => {
      const t = tasks.find(x => x.id === id)!;
      return api.put("/TaskTicket/Update", { ...t, archived: true });
    })).then(() => { setSel(new Set()); load(); });

  const bulkComplete = () =>
    Promise.all(Array.from(sel).map(id => {
      const t = tasks.find(x => x.id === id)!;
      return api.put("/TaskTicket/Update", { ...t, status: "Completed" });
    })).then(() => { setSel(new Set()); load(); });

  return (
    <>
      <AppBar position="static">
        <Toolbar>
          <Typography sx={{ flexGrow: 1 }}>Tickets</Typography>
          {isManager && (
            <IconButton color="inherit" onClick={() => setOpen(true)}><AddIcon /></IconButton>
          )}
          <IconButton color="inherit" onClick={logout}><LogoutIcon /></IconButton>
        </Toolbar>
      </AppBar>

      {isManager && sel.size > 0 &&
        <Stack direction="row" spacing={1} sx={{ m: 2 }}>
          <Button onClick={bulkComplete}>Complete ({sel.size})</Button>
          <Button onClick={bulkArchive}>Archive ({sel.size})</Button>
          <Checkbox
            checked={sel.size === tasks.length}
            indeterminate={sel.size > 0 && sel.size < tasks.length}
            onChange={(_, v) => setSel(v ? new Set(tasks.map(t => t.id)) : new Set())}
          />
          Select all
        </Stack>}

      <div style={{ maxWidth: 800, margin: "16px auto" }}>
        {tasks.filter(t => !t.archived).map(t => (
          <TicketCard key={t.id} t={t}
            selectable={isManager}
            isManager={isManager}
            onSelect={(id, c) => {
              const s = new Set(sel); c ? s.add(id) : s.delete(id); setSel(s);
            }}
            onAction={act => {
              if (act === "edit") { setEdit(t); setOpen(true); }
              if (act === "archive") api.put("/TaskTicket/Update", { ...t, archived: true }).then(load);
              if (act === "delete") api.delete(`/TaskTicket/Delete?id=${t.id}`).then(load);
              if (act === "done") api.put("/TaskTicket/Update", { ...t, status: "Completed" }).then(load);
              if (act === "start") api.put("/TaskTicket/Update", { ...t, status: "InProgress" }).then(load);
            }} />
        ))}
      </div>

      <TicketModal open={open} onClose={() => { setOpen(false); setEdit(null); }}
                   initial={edit} onSave={saveTask} />
    </>
  );
}    
