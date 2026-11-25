import { BrowserRouter, Routes, Route } from 'react-router-dom';
import HomePage from './pages/store/HomePage.jsx';
import LoginPage from './pages/auth/LoginPage.jsx';
import RegisterPage from './pages/auth/RegistrationPage.jsx';
import VerifyEmailPage from "./pages/auth/VerifyEmailPage.jsx";

function App() {
  return (
      <BrowserRouter>
        <Routes>
            <Route path="/" element={<HomePage />} />
            <Route path="/login" element={<LoginPage />} />
            <Route path="/registration" element={<RegisterPage />} />
            <Route path="/verify-email" element={<VerifyEmailPage />} />
        </Routes>
      </BrowserRouter>
  );
}

export default App;