/* src/pages/TasksPage/TicketCard.tsx */

import {
    Card, CardHeader, CardContent, Typography, Checkbox,
    IconButton, Button, Collapse, CircularProgress,
    List, ListItem, ListItemText, Box
  } from "@mui/material";
  import EditIcon      from "@mui/icons-material/Edit";
  import ArchiveIcon   from "@mui/icons-material/Archive";
  import DeleteIcon    from "@mui/icons-material/Delete";
  import DoneIcon      from "@mui/icons-material/Done";
  import PlayArrowIcon from "@mui/icons-material/PlayArrow";
  import HistoryIcon   from "@mui/icons-material/History";
  
  import { useState } from "react";
  import api from "../../api/http";
  import { TaskTicket, TaskHistory } from "./types";
  
  interface Props {
    t: TaskTicket;
    selectable?: boolean;
    onSelect?: (id: string, sel: boolean) => void;
    onAction: (act: "edit" | "delete" | "archive" | "start" | "done") => void;
    isManager: boolean;
  }
  
  export default function TicketCard({
    t, selectable, onSelect, onAction, isManager
  }: Props) {
    /* ------------ история ------------ */
    const [openHist, setOpenHist]   = useState(false);
    const [hist, setHist]           = useState<TaskHistory[] | null>(null);
  
    const toggleHist = () => {
      if (!openHist && hist === null) {
        api.get<TaskHistory[]>("/TaskHistory/ByTicket", { params: { ticketId: t.id } })
           .then(r => setHist(r.data))
           .catch(() => setHist([]));
      }
      setOpenHist(!openHist);
    };
  
    return (
      <Card sx={{ mb: 2, position: "relative", pl: selectable ? 5 : 0 }}>
        {/* чекбокс выбора */}
        {selectable && (
          <Checkbox
            sx={{ position: "absolute", left: 8, top: 12 }}
            onChange={e => onSelect?.(t.id, e.target.checked)}
          />
        )}
  
        {/* кнопка истории */}
        <IconButton
          onClick={toggleHist}
          sx={{ position: "absolute", right: 8, top: 8 }}
          title="History"
        >
          <HistoryIcon fontSize="small" />
        </IconButton>
  
        <CardHeader
          title={<Box sx={{ wordBreak: "break-word" }}>#{t.id.slice(0, 8)} {t.title}</Box>}
          subheader={t.priority}
          sx={{ pt: selectable ? 1 : 2 }}
        />
  
        <CardContent>
          <Typography variant="body2">{t.description}</Typography>
          <Typography variant="caption" display="block" sx={{ mt: 1 }}>
            Due: {new Date(t.due).toLocaleDateString()} &nbsp;|&nbsp; Status: {t.status}
          </Typography>
  
          <Box sx={{ mt: 1 }}>
            {isManager ? (
              <>
                <IconButton onClick={() => onAction("edit")}   ><EditIcon /></IconButton>
                <IconButton onClick={() => onAction("archive")}><ArchiveIcon /></IconButton>
                <IconButton onClick={() => onAction("delete")} ><DeleteIcon /></IconButton>
                {t.status !== "Completed" &&
                  <IconButton onClick={() => onAction("done")} ><DoneIcon /></IconButton>}
              </>
            ) : (
              t.status === "New" ? (
                <Button size="small" startIcon={<PlayArrowIcon />}
                        onClick={() => onAction("start")}>Start</Button>
              ) : t.status === "InProgress" && (
                <Button size="small" startIcon={<DoneIcon />}
                        onClick={() => onAction("done")}>Finish</Button>
              )
            )}
          </Box>
        </CardContent>
  
        {/* ---------- история ---------- */}
        <Collapse in={openHist} timeout="auto" unmountOnExit>
          <CardContent sx={{ bgcolor: "#f9f9f9", pt: 1 }}>
            {hist === null ? (
              <CircularProgress size={20} />
            ) : hist.length === 0 ? (
              <Typography variant="caption">No history</Typography>
            ) : (
              /* max-height: 200px ≈ ~10 записей; скроллим, если больше */
              <List dense sx={{ maxHeight: 200, overflowY: "auto" }}>
                {hist.slice(0, 50).map(h => (              
                  <ListItem key={h.id}>
                    <ListItemText
                      primary={`${new Date(h.at).toLocaleString()} — ${h.action}`}
                      secondary={`by ${h.by ?? "system"}`} />
                  </ListItem>
                ))}
              </List>
            )}
          </CardContent>
        </Collapse>
      </Card>
    );
  }
  