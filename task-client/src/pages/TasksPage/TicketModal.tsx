import {
    Dialog, DialogTitle, DialogContent, DialogActions,
    TextField, MenuItem, Button
  } from "@mui/material";
  import { useState, useEffect } from "react";
  import { TaskTicket, Priority } from "./types";
  
  interface Props {
    open: boolean;
    onClose: () => void;
    onSave: (patch: Partial<TaskTicket>) => void;
    initial?: TaskTicket | null;
  }
  
  export default function TicketModal({ open, onClose, onSave, initial }: Props) {
    const blank = {
      title: "", description: "", due: "", priority: "Medium" as Priority, assignee: "employee"
    };
    const [form, set] = useState(blank);
  
    useEffect(() => { initial ? set(initial) : set(blank); }, [initial]);
  
    const handleSave = () => { onSave(form as any); onClose(); };
  
    return (
      <Dialog open={open} onClose={onClose} fullWidth maxWidth="sm">
        <DialogTitle>{initial ? "Edit task" : "New task"}</DialogTitle>
        <DialogContent sx={{ display: "flex", flexDirection: "column", gap: 2, mt: 1 }}>
          <TextField label="Title" value={form.title}
                     onChange={e => set({ ...form, title: e.target.value })} />
          <TextField label="Description" multiline rows={3}
                     value={form.description}
                     onChange={e => set({ ...form, description: e.target.value })} />
          <TextField type="date" label="Due" InputLabelProps={{ shrink: true }}
                     value={form.due}
                     onChange={e => set({ ...form, due: e.target.value })} />
          <TextField select label="Priority" value={form.priority}
                     onChange={e => set({ ...form, priority: e.target.value as Priority })}>
            {["Low", "Medium", "High"].map(p => <MenuItem key={p} value={p}>{p}</MenuItem>)}
          </TextField>
        </DialogContent>
        <DialogActions>
          <Button onClick={onClose}>Cancel</Button>
          <Button onClick={handleSave} variant="contained">Save</Button>
        </DialogActions>
      </Dialog>
    );
  }
  