import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import AuthTemplate from './AuthTemplate';
import { register } from './auth.js';

export default function RegisterPage() {
    const [login, setLogin] = useState('');
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);

    const navigate = useNavigate();

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');
        setLoading(true);

        if (password !== confirmPassword) {
            setError('Пароли не совпадают');
            setLoading(false);
            return;
        }

        try {
            const result = await register(login, email, password);

            if (result.userId) {
                localStorage.setItem('pendingUserId', result.userId);
                navigate('/verify-email');
            } else {
                setError(result.message || 'Ошибка регистрации');
            }
        } catch (err) {
            console.error(err);
            setError('Ошибка сервера');
        } finally {
            // 3. ВСЕГДА снимаем блокировку, даже если упала ошибка
            setLoading(false);
        }
    };

    return (
        <AuthTemplate title="Регистрация">
            <form onSubmit={handleSubmit}>
                <div>
                    <h3>Email</h3>
                    <input
                        type="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        required
                    />
                </div>
                <div>
                    <h3>Логин</h3>
                    <input
                        type="text"
                        value={login}
                        onChange={(e) => setLogin(e.target.value)}
                        required
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
                <div>
                    <h3>Повторите пароль</h3>
                    <input
                        type="password"
                        value={confirmPassword}
                        onChange={(e) => setConfirmPassword(e.target.value)}
                        required
                    />
                </div>

                {error && <p className="error-msg">{error}</p>}

                <button type="submit" disabled={loading}>
                    {loading ? 'Загрузка...' : 'Зарегистрироваться'}
                </button>

                <div>
                    <Link to="/login">Уже зарегистрированы?</Link>
                </div>
            </form>
        </AuthTemplate>
    );
}