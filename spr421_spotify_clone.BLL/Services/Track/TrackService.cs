using AutoMapper;
using Microsoft.EntityFrameworkCore;
using spr421_spotify_clone.BLL.Dtos.Track;
using spr421_spotify_clone.BLL.Services.Storage;
using spr421_spotify_clone.DAL.Entities;
using spr421_spotify_clone.DAL.Repositories.Genre;
using spr421_spotify_clone.DAL.Repositories.Track;
using System.Net;

namespace spr421_spotify_clone.BLL.Services.Track
{
    public class TrackService : ITrackService
    {
        private readonly ITrackRepository _trackRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly IStorageService _storageService;
        private readonly IMapper _mapper;

        public TrackService(ITrackRepository trackRepository, IMapper mapper, IGenreRepository genreRepository, IStorageService storageService)
        {
            _trackRepository = trackRepository;
            _mapper = mapper;
            _genreRepository = genreRepository;
            _storageService = storageService;
        }

        public async Task<ServiceResponse> CreateAsync(CreateTrackDto dto, string audioFilePath, string posterFilePath)
        {
            var entity = _mapper.Map<TrackEntity>(dto);

            var genre = await _genreRepository.GetByIdAsync(dto.GenreId);

            if(genre == null)
            {
                return new ServiceResponse
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.NotFound,
                    Message = $"Жанр з id '{dto.GenreId}' не знайдено"
                };
            }

            entity.Genre = genre;

            // Save audio file
            var audioFileName = await _storageService.SaveAudioFileAsync(dto.AudioFile, audioFilePath);

            if(audioFileName == null)
            {
                return new ServiceResponse
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Не вдалося зберегти аудіо"
                };
            }
            entity.AudioUrl = audioFileName;

            // Save poster file
            if(dto.PosterFile != null)
            {
                var posterFileName = await _storageService.SaveImageFileAsync(dto.PosterFile, posterFilePath);
                if (posterFileName == null)
                {
                    return new ServiceResponse
                    {
                        IsSuccess = false,
                        StatusCode = HttpStatusCode.BadRequest,
                        Message = "Не вдалося зберегти зображення"
                    };
                }
                entity.PosterUrl = posterFileName;
            }

            await _trackRepository.CreateAsync(entity);

            return new ServiceResponse
            {
                Message = $"Трек '{entity.Title}' додано"
            };
        }

        public async Task<ServiceResponse> GetAllAsync()
        {
            var entities = await _trackRepository.Tracks
                .Include(t => t.Genre)
                .ToListAsync();

            var dtos = _mapper.Map<List<TrackDto>>(entities);

            return new ServiceResponse
            {
                Message = "Треки отримано",
                Payload = dtos
            };
        }

        public async Task<ServiceResponse> RemoveAsync(string id, string audioFilePath)
        {
            var entity = await _trackRepository.GetByIdAsync(id);

            if(entity == null)
            {
                return new ServiceResponse
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.NotFound,
                    Message = $"Трек з id '{id}' не знайдено"
                };
            }

            audioFilePath = Path.Combine(audioFilePath, entity.AudioUrl);

            File.Delete(audioFilePath);
            await _trackRepository.DeleteAsync(entity);

            return new ServiceResponse
            {
                Message = $"Трек '{entity.Title}' видалено"
            };
        }
    }
}
