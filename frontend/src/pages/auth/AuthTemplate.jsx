import { Link } from 'react-router-dom';

export default function AuthTemplate({ children, title }) {
    return (
        <div className="auth-template">
            <Link to="/" className="auth-template__logo">
                VynilServer
            </Link>

            <div className="auth-template__content">
                {title && <h1>{title}</h1>}
                {children}
            </div>

            <Link to="/" className="auth-template__back">
                <button>Вернуться в магазин</button>
            </Link>
        </div>
    );
}