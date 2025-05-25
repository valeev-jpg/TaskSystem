import { useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  TextField, Button, Card, CardContent, Typography,
  CircularProgress, Dialog, DialogTitle, DialogContent, DialogActions
} from "@mui/material";
import { useAuth } from "../auth/AuthContext";

export default function LoginPage() {
  const { login } = useAuth();
  const nav = useNavigate();
  const [user, setUser] = useState("");
  const [pass, setPass] = useState("");
  const [err, setErr]   = useState("");
  const [loading, setLoading] = useState(false);
  const [openSmartcard, setOpenSmartcard] = useState(false); // <-- для модального окна

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

  const handleSmartcardLogin = () => {
    setOpenSmartcard(true);
  };

  const handleSmartcardSubmit = () => {
    // Здесь можно вставить реальную проверку смарт-карты
    alert("Смарт-карта проверяется...");
    setOpenSmartcard(false);
  };

  return (
    <>
      <Card sx={{ maxWidth: 320, m: "auto", mt: 12, p: 2 }}>
        <CardContent component="form" onSubmit={handle}>
          <Typography variant="h6" align="center" gutterBottom>Авторизация</Typography>
          {err && <Typography color="error">{err}</Typography>}
          <TextField
            label="Логин"
            fullWidth
            margin="normal"
            value={user}
            onChange={e => setUser(e.target.value)}
          />
          <TextField
            label="Пароль"
            type="password"
            fullWidth
            margin="normal"
            value={pass}
            onChange={e => setPass(e.target.value)}
          />
          <Button
            type="submit"
            fullWidth
            variant="contained"
            disabled={loading}
            sx={{ mt: 2 }}
          >
            {loading && <CircularProgress size={18} sx={{ mr:1 }} />} Вход
          </Button>
          <Button
            fullWidth
            variant="outlined"
            sx={{ mt: 1 }}
            onClick={handleSmartcardLogin}
          >
            Вход по смарт-карте
          </Button>
        </CardContent>
      </Card>

      {/* Модальное окно для смарт-карты */}
      <Dialog open={openSmartcard} onClose={() => setOpenSmartcard(false)}>
        <DialogTitle>Авторизация по смарт-карте</DialogTitle>
        <DialogContent>
          <Typography>Пожалуйста, вставьте вашу смарт-карту</Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenSmartcard(false)}>Отмена</Button>
          <Button variant="contained" onClick={handleSmartcardSubmit}>Войти</Button>
        </DialogActions>
      </Dialog>
    </>
  );
}
