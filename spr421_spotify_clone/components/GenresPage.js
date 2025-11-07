import React, { useState, useEffect } from 'react';
import axios from 'axios';

const GenresPage = () => {
    const [genres, setGenres] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    const API_BASE_URL = 'http://localhost:8080/api';

    useEffect(() => {
        fetchGenres();
    }, []);

    const fetchGenres = async () => {
        try {
            setLoading(true);
            const response = await axios.get(`${API_BASE_URL}/genres`);
            setGenres(response.data);
            setError('');
        } catch (err) {
            setError('Помилка при завантаженні жанрів');
            console.error('Error fetching genres:', err);
        } finally {
            setLoading(false);
        }
    };

    if (loading) {
        return (
            <div className="container mt-4">
                <div className="d-flex justify-content-center">
                    <div className="spinner-border" role="status">
                        <span className="visually-hidden">Завантаження...</span>
                    </div>
                </div>
            </div>
        );
    }

    return (
        <div className="container mt-4">
            <div className="row">
                <div className="col-12">
                    <h1 className="mb-4">Жанри</h1>

                    {error && (
                        <div className="alert alert-danger" role="alert">
                            {error}
                        </div>
                    )}

                    <div className="row">
                        {genres.length > 0 ? (
                            genres.map(genre => (
                                <div key={genre.id} className="col-md-4 mb-3">
                                    <div className="card">
                                        <div className="card-body">
                                            <h5 className="card-title">{genre.name}</h5>
                                            {genre.description && (
                                                <p className="card-text">{genre.description}</p>
                                            )}
                                        </div>
                                    </div>
                                </div>
                            ))
                        ) : (
                            <div className="col-12">
                                <div className="alert alert-info" role="alert">
                                    Жанри не знайдено
                                </div>
                            </div>
                        )}
                    </div>

                    <button
                        className="btn btn-primary mt-3"
                        onClick={fetchGenres}
                    >
                        Оновити список
                    </button>
                </div>
            </div>
        </div>
    );
};

export default GenresPage;