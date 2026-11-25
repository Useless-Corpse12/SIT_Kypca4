import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import AuthTemplate from './AuthTemplate';
import { login } from './auth';

export default function LoginPage() {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');
        setLoading(true);

        try {
            const result = await login(email, password);

            if (result.success) {
                navigate('/');
            } else if (result.requires2FA) {
                navigate('/verify-login');
            } else if (result.unverified) {
                setError('Аккаунт не подтверждён. Проверьте почту.');
            } else {
                setError('Неверный email или пароль');
            }
        } catch {
            setError('Ошибка сервера');
        } finally {
            setLoading(false);
        }
    };

    return (
        <AuthTemplate title="Авторизация">
            <form onSubmit={handleSubmit}>
                <div>
                    <h3>Email</h3>
                    <input
                        type="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        required
                        autoFocus
                    />
                </div>
                <div>
                    <h3>Пароль</h3>
                    <input
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        required
                    />
                </div>

                {error && <p>{error}</p>}

                <button type="submit" disabled={loading}>
                    {loading ? 'Входим...' : 'Войти'}
                </button>
                <div>
                    <Link to={'/registration'}>
                        Нет аккаунта?
                    </Link>
                </div>
            </form>
        </AuthTemplate>
    );
}