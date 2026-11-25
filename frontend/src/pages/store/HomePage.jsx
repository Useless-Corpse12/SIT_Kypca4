import { Link } from 'react-router-dom';

export default function HomePage() {
    return (
        <div>
            <h1>Главная страница</h1>
            {/* Link ведет себя как ссылка, но внутри можно стилизовать под кнопку */}
            <Link to="/login">
                <button>Войти</button>
            </Link>
        </div>
    );
}