import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import AuthTemplate from './AuthTemplate';
import { verifyEmail, resendCode } from './auth';

export default function VerifyEmailPage() {
    const [code, setCode] = useState('');
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();


    const userId = localStorage.getItem('pendingUserId');

    const handleVerify = async (e) => {
        e.preventDefault();
        setError('');
        setLoading(true);

        if (!userId) {
            setError('Ошибка сессии. Пройдите регистрацию заново.');
            setLoading(false);
            return;
        }

        try {
            const result = await verifyEmail(userId, code);


            if (result.success) {
                localStorage.removeItem('pendingUserId');
                navigate('/login');
            } else {
                setError(result.message || 'Неверный код');
            }
        } catch {
            setError('Ошибка сервера');
        } finally {
            setLoading(false);
        }
    };

    const handleResend = async () => {
        if (!userId) return;
        setLoading(true);
        try {
            const result = await resendCode(userId);
            if (result.isSuccess) {
                setError('Код отправлен повторно');
            } else {
                setError(result.message || 'Не удалось отправить код');
            }
        } catch {
            setError('Ошибка сервера');
        } finally {
            setLoading(false);
        }
    };

    return (
        <AuthTemplate title="Подтверждение почты">
            <form onSubmit={handleVerify} className="auth-form">
                <div className="field">
                    <h3>Код из письма</h3>
                    <input
                        className="input"
                        type="text"
                        value={code}
                        onChange={(e) => setCode(e.target.value)}
                        placeholder="123456"
                        required
                        autoFocus
                    />
                </div>

                {error && <p className="error-msg">{error}</p>}

                <button className="btn btn-primary" type="submit" disabled={loading}>
                    {loading ? 'Проверка...' : 'Подтвердить'}
                </button>

                <button
                    type="button"
                    className="link"
                    onClick={handleResend}
                    disabled={loading}
                >
                    Отправить код повторно
                </button>
            </form>
        </AuthTemplate>
    );
}