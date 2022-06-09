namespace MusicHub
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Data;
    using Initializer;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            MusicHubDbContext context = 
                new MusicHubDbContext();

            DbInitializer.ResetDatabase(context);


            string res = ExportSongsAboveDuration(context , 4);
            Console.WriteLine(res);
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {

            StringBuilder sb = new StringBuilder();


            var albumsInfo = context.Albums
                .ToArray()
                .Where(a => a.ProducerId == producerId)
                 .OrderByDescending(a => a.Price)
                .Select(a => new
                {
                    Name = a.Name,
                    ReleaseDate = a.ReleaseDate.ToString("MM/dd/yyyy"),
                    ProducerName = a.Producer.Name,
                    Songs = a.Songs.Select(s => new
                    {
                        SongName = s.Name,
                        SongPrice = s.Price.ToString("f2"), 
                        SongWriterName = s.Writer.Name,
                    }).OrderByDescending(s => s.SongName)
                    .ThenBy(s => s.SongWriterName)
                    .ToList(),
                    TotalAlbumPrice = a.Price.ToString("f2")
                })
                .ToList();


            foreach (var album in albumsInfo)
            {
                sb
                    .AppendLine($"-AlbumName: {album.Name}")
                    .AppendLine($"-ReleaseDate: {album.ReleaseDate}")
                    .AppendLine($"-ProducerName: {album.ProducerName}")
                    .AppendLine("-Songs:");

                int i = 1;
                foreach (var song in album.Songs)
                {
                    sb
                        .AppendLine($"---#{i++}")
                        .AppendLine($"---SongName: {song.SongName}")
                        .AppendLine($"---Price: {song.SongPrice}")
                        .AppendLine($"---Writer: {song.SongWriterName}");
                }

                sb.AppendLine($"-AlbumPrice: {album.TotalAlbumPrice}");
            }

            return sb.ToString().TrimEnd();

        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            StringBuilder sb = new StringBuilder();

            var songs = context.Songs
                .ToArray()
                .Where(s => s.Duration.TotalSeconds > duration)
                .Select(s => new
                {
                    SongName = s.Name, 
                    Writer = s.Writer.Name,
                    PerformerFullname = s.SongPerformers
                    .ToArray()
                    .Select(sp => $"{sp.Performer.FirstName} {sp.Performer.LastName}")  
                    .FirstOrDefault(), 
                    AlbumProducer = s.Album.Producer.Name,
                    Duration = s.Duration.ToString("c" , CultureInfo.InvariantCulture)
                })
                .OrderBy(s => s.SongName)
                .ThenBy(s => s.Writer)
                .ThenBy(s => s.PerformerFullname)
                .ToList();

            int i = 1;
            foreach (var song in songs)
            {
                sb
                    .AppendLine($"-Song #{i++}")
                    .AppendLine($"---SongName: {song.SongName}")
                    .AppendLine($"---Writer: {song.Writer}")
                    .AppendLine($"---Performer: {song.PerformerFullname}")
                    .AppendLine($"---AlbumProducer: {song.AlbumProducer}")
                    .AppendLine($"---Duration: {song.Duration}");
            }

            return sb.ToString().TrimEnd();
        }
    }
}
