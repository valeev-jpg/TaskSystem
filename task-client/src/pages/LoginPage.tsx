import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { TextField, Button, Card, CardContent, Typography, CircularProgress } from "@mui/material";
import { useAuth } from "../auth/AuthContext";

export default function LoginPage() {
  const { login } = useAuth();
  const nav = useNavigate();
  const [user, setUser] = useState("User1");
  const [pass, setPass] = useState("password");
  const [err, setErr]   = useState("");
  const [loading, setLoading] = useState(false);

  const handle = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    try {
      await login(user, pass);
      nav("/");
    } catch (ex: any) {
      setErr(ex.message ?? "Auth failed");
    } finally {
      setLoading(false);
    }
  };

  return (
    <Card sx={{ maxWidth: 320, m: "auto", mt: 12, p: 2 }}>
      <CardContent component="form" onSubmit={handle}>
        <Typography variant="h6" align="center" gutterBottom>Login</Typography>
        {err && <Typography color="error">{err}</Typography>}
        <TextField label="Username" fullWidth margin="normal" value={user} onChange={e=>setUser(e.target.value)} />
        <TextField label="Password" type="password" fullWidth margin="normal" value={pass} onChange={e=>setPass(e.target.value)} />
        <Button type="submit" fullWidth variant="contained" disabled={loading}>
          {loading && <CircularProgress size={18} sx={{ mr:1 }} />} Login
        </Button>
      </CardContent>
    </Card>
  );
}
