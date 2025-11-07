import React, { useState, useEffect } from 'react';
import axios from 'axios';

const AddTrackPage = () => {
const [genres, setGenres] = useState([]);
const [loading, setLoading] = useState(false);
const [success, setSuccess] = useState('');
const [error, setError] = useState('');

const [formData, setFormData] = useState({
title: '',
        artist: '',
        album: '',
        duration: '',
        genreId: '',
        releaseDate: ''
    });

const [audioFile, setAudioFile] = useState(null);

const API_BASE_URL = 'http://localhost:8080/api';

useEffect(() => {
    fetchGenres();
}, []);

const fetchGenres = async () => {
    try
    {
        const response = await axios.get(`${ API_BASE_URL}/ genres`);
        setGenres(response.data);
    }
    catch (err)
    {
        console.error('Error fetching genres:', err);
    }
};

const handleInputChange = (e) => {
const { name, value } = e.target;
setFormData(prev => ({
    ...prev,
            [name]: value
        }));
    };

const handleFileChange = (e) => {
    setAudioFile(e.target.files[0]);
};

const handleSubmit = async (e) => {
e.preventDefault();

if (!audioFile)
{
    setError('Будь ласка, виберіть аудіо файл');
    return;
}

if (!formData.genreId)
{
    setError('Будь ласка, виберіть жанр');
    return;
}

try
{
    setLoading(true);
    setError('');
    setSuccess('');

    // Створюємо FormData для відправки файлу
    const submitData = new FormData();
    submitData.append('audioFile', audioFile);
    submitData.append('title', formData.title);
    submitData.append('artist', formData.artist);
    submitData.append('album', formData.album || '');
    submitData.append('duration', formData.duration || '0');
    submitData.append('genreId', formData.genreId);
    submitData.append('releaseDate', formData.releaseDate || '');

    const response = await axios.post(`${ API_BASE_URL}/ tracks`, submitData, {
    headers:
        {
            'Content-Type': 'multipart/form-data',
                },
            });

setSuccess('Трек успішно додано!');

// Очищаємо форму
setFormData({
title: '',
                artist: '',
                album: '',
                duration: '',
                genreId: '',
                releaseDate: ''
            });
setAudioFile(null);
document.getElementById('audioFile').value = '';

        } catch (err) {
    setError('Помилка при додаванні треку: ' + (err.response?.data?.message || err.message));
    console.error('Error adding track:', err);
} finally {
    setLoading(false);
}
    };

return (

    < div className = "container mt-4" >

        < div className = "row" >

            < div className = "col-12" >

                < h1 className = "mb-4" > Додати новий трек</h1>

                    {
    success && (
                        <div className="alert alert-success" role="alert">
                            { success}
                        </div>
                    )}

{
    error && (

    < div className = "alert alert-danger" role = "alert" >
                            { error}
                        </ div >
                    )}

                    < form onSubmit ={ handleSubmit}>
                        < div className = "row" >
                            < div className = "col-md-6" >
                                < div className = "mb-3" >
                                    < label htmlFor = "title" className = "form-label" >
                                        Назва треку *
                                    </ label >
                                    < input
                                        type = "text"
                                        className = "form-control"
                                        id = "title"
                                        name = "title"
                                        value ={ formData.title}
onChange ={ handleInputChange}
required
/>

</ div >


< div className = "mb-3" >

< label htmlFor = "artist" className = "form-label" >
Виконавець *

</ label >

< input
                                        type = "text"
                                        className = "form-control"
                                        id = "artist"
                                        name = "artist"
                                        value ={ formData.artist}
onChange ={ handleInputChange}
required
/>

</ div >


< div className = "mb-3" >

< label htmlFor = "album" className = "form-label" >
Альбом
</ label >

< input
                                        type = "text"
                                        className = "form-control"
                                        id = "album"
                                        name = "album"
                                        value ={ formData.album}
onChange ={ handleInputChange}
                                    />
                                </ div >
                            </ div >

                            < div className = "col-md-6" >
                                < div className = "mb-3" >
                                    < label htmlFor = "duration" className = "form-label" >
                                        Тривалість(секунди)
                                    </ label >
                                    < input
                                        type = "number"
                                        className = "form-control"
                                        id = "duration"
                                        name = "duration"
                                        value ={ formData.duration}
onChange ={ handleInputChange}
min = "0"
/>

</ div >


< div className = "mb-3" >

< label htmlFor = "genreId" className = "form-label" >
Жанр *

</ label >

< select
                                        className = "form-select"
                                        id = "genreId"
                                        name = "genreId"
                                        value ={ formData.genreId}
onChange ={ handleInputChange}
required
>

< option value = "" > Виберіть жанр </ option >
                                        {
    genres.map(genre => (
                                            < option key ={ genre.id}
    value ={ genre.id}>
                                                { genre.name}
                                            </ option >
                                        ))}
                                    </ select >
                                </ div >

                                < div className = "mb-3" >
                                    < label htmlFor = "releaseDate" className = "form-label" >
                                        Дата релізу
                                    </ label >
                                    < input
                                        type = "date"
                                        className = "form-control"
                                        id = "releaseDate"
                                        name = "releaseDate"
                                        value ={ formData.releaseDate}
onChange ={ handleInputChange}
                                    />
                                </ div >
                            </ div >
                        </ div >

                        < div className = "mb-3" >
                            < label htmlFor = "audioFile" className = "form-label" >
                                Аудіо файл *
                            </ label >
                            < input
                                type = "file"
                                className = "form-control"
                                id = "audioFile"
                                accept = "audio/*"
                                onChange ={ handleFileChange}
required
/>

< div className = "form-text" >
Оберіть аудіо файл у форматі MP3, WAV або іншому підтримуваному форматі
                            </div>
                        </div>

                        {audioFile && (
                            <div className="mb-3">
                                <strong>Обраний файл:</ strong > { audioFile.name}
                            </ div >
                        )}

                        < button
                            type = "submit"
                            className = "btn btn-primary"
                            disabled ={ loading}
                        >
                            {
    loading ? (
                                <>
                                    < span className = "spinner-border spinner-border-sm me-2" role = "status" ></ span >
                                    Додавання...
                                </>
                            ) : (
                                'Додати трек'
                            )}
                        </ button >
                    </ form >
                </ div >
            </ div >
        </ div >
    );
};

export default AddTrackPage;