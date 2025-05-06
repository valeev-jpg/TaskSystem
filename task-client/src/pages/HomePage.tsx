import { useEffect, useState } from "react";
import { Typography, IconButton, List, ListItem, ListItemText, AppBar, Toolbar } from "@mui/material";
import LogoutIcon from "@mui/icons-material/Logout";
import api from "../api/http";
import { useAuth } from "../auth/AuthContext";

interface Server { id: string; name: string }

export default function HomePage() {
  const { logout } = useAuth();
  const [servers, setServers] = useState<Server[]>([]);
  const [err, setErr] = useState("");

  useEffect(() => {
    api.get<Server[]>("/Server/GetAll")
       .then(r => setServers(r.data))
       .catch(e => setErr(e.message));
  }, []);

  return (
    <>
      <AppBar position="static">
        <Toolbar>
          <Typography variant="h6" sx={{ flexGrow: 1 }}>Servers</Typography>
          <IconButton color="inherit" onClick={logout}><LogoutIcon/></IconButton>
        </Toolbar>
      </AppBar>
      {err ? <Typography color="error" sx={{ m:2 }}>{err}</Typography> :
        <List>
          {servers.map(s => <ListItem key={s.id}><ListItemText primary={s.name}/></ListItem>)}
        </List>}
    </>
  );
}
