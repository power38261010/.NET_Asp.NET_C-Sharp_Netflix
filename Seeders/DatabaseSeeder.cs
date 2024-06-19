using Microsoft.EntityFrameworkCore;
using NetflixClone.Data;
using NetflixClone.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NetflixClone.Seeders
{
    public static class DatabaseSeeder
    {
        public static async Task Seed(ApplicationDbContext context)
        {
            static string HashPassword(string password)
            {
                using (var sha256 = SHA256.Create())
                {
                    var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                    return Convert.ToBase64String(hashedBytes);
                }
            }
            // Ensure database is created
            context.Database.EnsureCreated();

            // Seed subscriptions
            var subscriptions = new List<Subscription>
            {
                new Subscription { Type = "Started" },
                new Subscription { Type = "Premium" }
            };

            foreach (var subscription in subscriptions)
            {
                if (!context.Subscriptions.Any(s => s.Type == subscription.Type))
                {
                    context.Subscriptions.Add(subscription);
                }
            }
            await context.SaveChangesAsync();

            // Fetch the saved subscriptions from the context to get their IDs
            var savedSubscriptions = context.Subscriptions.ToList();

            // Seed payments
            var payments = new List<Pay>
            {
                new Pay { Currency = "USD", MonthlyPayment = (decimal)9.99m, SubscriptionId = savedSubscriptions.First(s => s.Type == "Started").Id },
                new Pay { Currency = "USD", MonthlyPayment = (decimal)19.99m, SubscriptionId = savedSubscriptions.First(s => s.Type == "Premium").Id }
            };

            foreach (var pay in payments)
            {
                if (!context.Payments.Any(p => p.MonthlyPayment == pay.MonthlyPayment && p.SubscriptionId == pay.SubscriptionId && p.Currency == pay.Currency))
                {
                    context.Payments.Add(pay);
                }
            }
            await context.SaveChangesAsync();

            // Seed users
            var users = new List<User>
            {
                new User { Username = "super_admin", PasswordHash = HashPassword("admin1234"), Role = "super_admin" },
                new User { Username = "soy_admin", PasswordHash = HashPassword("admin1234"), Role = "admin" },
                new User { Username = "fran_duti", PasswordHash = HashPassword("password123"), Role = "client", SubscriptionId = 1 },
                new User { Username = "Fer_nando", PasswordHash = HashPassword("password123"), Role = "client", SubscriptionId = 1 },
                new User { Username = "Edgar_Watcher", PasswordHash = HashPassword("password123"), Role = "client", SubscriptionId = 1 },
                new User { Username = "Marieclarie", PasswordHash = HashPassword("password123"), Role = "client", SubscriptionId = 1 },
                new User { Username = "Maria", PasswordHash = HashPassword("password123"), Role = "client", SubscriptionId = 1 },
                new User { Username = "Jose", PasswordHash = HashPassword("password123"), Role = "client", SubscriptionId = 2 },
                new User { Username = "Paulo", PasswordHash = HashPassword("password123"), Role = "client", SubscriptionId = 2 },
                new User { Username = "Justin", PasswordHash = HashPassword("password123"), Role = "client", SubscriptionId = 2 },
                new User { Username = "movies-netflix", PasswordHash = HashPassword("password123"), Role = "client", SubscriptionId = 2 },
                new User { Username = "Selena", PasswordHash = HashPassword("password123"), Role = "client", SubscriptionId = 2 }
            };
            foreach (var user in users)
            {
                if (!context.Users.Any(u => u.Username == user.Username))
                {
                    context.Users.Add(user);
                }
            }
            await context.SaveChangesAsync();

            // Seed movies
            var movies = new List<Movie>
            {
                new Movie { Title = "EL PADRINO I",Rating = 7.2f,
                Description = " En los años 40, cinco familias pertenecientes a la Cosa Nostra italiana manejan Nueva York a su gusto. Todas las actividades al margen de la ley son gerenciadas por ellos y generan millonarias sumas para la organización.\n Pero la llegada de un nuevo negocio, el tráfico de estupefacientes, hace que Vito Corleone, capo de una de las cinco familias, se oponga rotundamente a entrar en ese tema. La consecuencia inmediata es un atentado en el que Don Corleone se salva por muy poco, pero queda gravemente herido. \n Es entonces cuando Michael Corleone, su hijo y quien siempre se mantuvo alejado de los negocios de la familia, decide tomar las riendas de la venganza y preparar el contraataque para mostrar que es digno heredero del clan. Ganadora de tres premios Oscar por: Película, Guión Adaptado y Actor Protagónico, por la deslumbrante actuación de Marlon Brando.",
                Genre = "Drama Crimen", ReleaseDate = DateTime.Now.AddYears(-1),
                PosterUrl="https ://www.clarin.com/img/2021/01/16/1tYRBDV7t_1256x620__1.jpg",
                TrailerUrl="https://www.youtube.com/watch?v=gCVj1LeYnsc&ab_channel=trailersinfojet", },

                new Movie { Title = "BATMAN: EL CABALLERO DE LA NOCHE",Rating = 7.2f,
                Description = "Batman mantiene el equilibrio en Ciudad Gótica con la ayuda del teniente James Gordon y el Fiscal de Distrito Harvey Dent. La unión de los tres es eficaz, pero la llegada de un nuevo villano llamado El Guasón (The Joker), pone en jaque la paz lograda y lleva a la ciudad a la anarquía y el caos. \n Entonces 'El Caballero de la noche' saldrá a combatirlo para recuperar la tranquilidad de los ciudadanos. Inolvidable por su gran guión, enormes actuaciones y la construcción de un sorprendente Guasón en manos del actor Heath Ledger (Secreto en la Montaña), en lo que sería su Oscar post mortem como actor de reparto. La película también ganó el Oscar a la mejor edición de sonido.",
                Genre = "Accion Drama", ReleaseDate = DateTime.Now.AddYears(-2),
                PosterUrl="https://www.clarin.com/img/2020/09/19/ooDWnM1tE_1256x620__1.jpg",
                TrailerUrl="https://www.youtube.com/watch?v=dzQtWkpc2-c&ab_channel=Recomiendo%23Cine"},

                new Movie { Title = "LA LISTA DE SCHINDLER", Rating = 7.2f,
                Description = "En plena Segunda Guerra Mundial, gracias a su filiación al partido nazi y a sus relaciones con jerarcas militares, el empresario alemán Oskar Schindler (Liam Neeson) se adueña de una fábrica. Pero el avance de los acontecimientos le hace comprender a Oskar que los miembros de su partido son asesinos de judíos y descubre que los empleados que contrata para la fábrica son salvados de ir a un campo de concentración. Así, con la ayuda de Itzhak Stern (Ben Kingsley), su gerente, intentará salvar a la mayor cantidad de personas posibles, aun arriesgando su propia vida.\n Basada en una historia real, está considerada una obra maestra de la historia del cine y le dio al notable director Steven Spielberg una altura comparable solo con los grandes realizadores.\n ​Ralph Fiennes como un despiadado criminal nazi, completa un triángulo actoral notable. Ganadora de siete premios Oscar: Mejor Película, Mejor Dirección, Mejor Guión Adaptado, Mejor Fotografía, Mejor Banda Sonora, Mejor Montaje y Mejor Dirección Artística.",
                Genre = "Accion Drama Belico", ReleaseDate = DateTime.Now.AddYears(-3),
                PosterUrl="https://www.clarin.com/img/2021/01/16/6P88lIhGV_1256x620__1.jpg",
                TrailerUrl="https://www.youtube.com/watch?v=BmkchuRJ82w&ab_channel=UniversalSpain"},

                new Movie { Title = "FORREST GUMP", Rating = 7.2f,
                Description = "Forrest es un joven con una pequeña incapacidad intelectual. Perseverante e inocente, y siguiendo los preceptos de superación inculcados por su madre, se convierte en protagonista inconsciente de la historia de los Estados Unidos. El camino lo recorrerá acompañado siempre por el amor eterno que le profesa a su amiga Jenny.\n Entrañable actuación de un Tom Hanks que no pararía de sorprender a la industria y que ganaría con esta actuación su segundo Oscar consecutivo a mejor actor protagónico, luego del obtenido en 1993 por Philadelphia. Además de su premio, Forrest Gump se alzó con otras cinco estatuillas: Mejor Película, Mejor Director, Mejor Guión Adaptado, Mejores Efectos Especiales y Mejor Montaje.",
                Genre = "Comedia Drama", ReleaseDate = DateTime.Now.AddYears(-4),
                PosterUrl="https://www.clarin.com/img/2018/06/22/BkdVqbsbm_1256x620__1.jpg",
                TrailerUrl="https://www.youtube.com/watch?v=GIs2gpWpBiQ&ab_channel=JennyAvila"},

                new Movie { Title = "EL ORIGEN", Rating = 7.2f,
                Description = "En un viaje hacia la redención, Dom Cobb -ladrón de sueños y secretos del subconsciente-, intenta recuperar los vínculos perdidos por culpa de su habilidad sobrenatural. Interpretado por Leonardo DiCaprio, el personaje llena le película y atrapa a los espectadores. Su enorme habilidad lo ha convertido en una estrella en el mundo del espionaje, pero debe vivir como un fugitivo, lejos de la tranquilidad de una vida placentera. \n Busca su oportunidad, pero una misión casi imposible lo desviará del camino y lo enfrentará a un enemigo que le lleva ventaja. La película ganó cuatro Oscars: Mejor Fotografía, Mejores Efectos Visuales, Mejor Sonido y Mejor Edición de Sonido.",
                Genre = "Ciencia-Ficcion Drama", ReleaseDate = DateTime.Now.AddYears(-5),
                PosterUrl="https://www.clarin.com/img/2021/01/16/0IXZUOR3R_1256x620__1.jpg",
                TrailerUrl="https://www.youtube.com/watch?v=BZOMKZ0AwC8&t=5s&ab_channel=AchoStudiosPeru"},

                new Movie { Title = "MATRIX", Rating = 7.2f,
                Description = "Neo (un enigmático y muy logrado papel de Keanu Reeves), es un experto hacker en un mundo cibernético donde las computadoras tienen el dominio de la humanidad. Es convocado por un grupo de outsiders del sistema llamado la Resistencia y liderado por Morfeo (Laurence Fishburne). \n Acompañado de Trinity (Carrie-Anne Moss), la enviada de Morfeo, Neo es inducido mediante una pastilla a un sistema digital informático para rescatar al líder de la Resistencia. Y, además, demostrar que él es el Elegido protector de la raza, el ciber-mesías. Ganadora de cuatro Oscars: Mejor Montaje, Mejor Sonido, Mejores Efectos Visuales, Mejores Efectos Sonoros.",
                Genre = "Ciencia-Ficcion Acción", ReleaseDate = DateTime.Now.AddYears(-1),
                PosterUrl="https://www.clarin.com/img/2019/03/29/IUGz0SY4o_1256x620__1.jpg",
                TrailerUrl="https://www.youtube.com/watch?v=Pl_H2Lmjn6k&ab_channel=IslaCalavera"},

                new Movie { Title = "PARÁSITE", Rating = 7.2f,
                Description = "Los Ki son una familia trabajadora de Seúl que apenas sobrevive en las periferias de la cuidad. El padre de la familia es Ki Taek, quien comienza a sentir que todo puede cambiar cuando su hijo mayor consigue trabajo como profesor de inglés en la casa de los Park, una familia acaudalada. \n Gracias al ingenio y la avidez, de a poco del muchacho irá incorporando como personal doméstico de los Park al resto de su familia. Esto mostrará que, pese a sus diferencias sociales, no son tan distintos, pero los resultados serán alocadamente inesperados. Ganadora de cuatro premios Oscar: Mejor Película Internacional, Mejor Director, Mejor Guión Original y Mejor Película.",
                Genre = "Drama Suspenso", ReleaseDate = DateTime.Now.AddYears(-2),
                PosterUrl="https://indiehoy.com/wp-content/uploads/2020/09/parasite-pelicula.jpg",
                TrailerUrl="https://www.youtube.com/watch?v=SEUXfv87Wpk&ab_channel=MadmanFilms"},

                new Movie { Title = "RESCATANDO AL SOLDADO RYAN", Rating = 7.2f,
                Description = "Tras el desembarco en Normandía, Francia, una patrulla de soldados Aliados liderados por el capitán John Miller (muy buen papel de Tom Hanks) es enviada a una difícil misión: rescatar al cuarto hermano de la familia Ryan ya que los otros tres fueron muertos en combate en esa misma guerra. \n ​Arriesgando sus vidas, este grupo de héroes emprenderá un viaje hacia lo más crudo de la guerra para cumplir con éxito su objetivo. Otra maravilla cinematográfica de Steven Spielberg. Ganadora de cinco Oscars: Mejor Director, Mejor Fotografía, Mejor Montaje, Mejor Sonido, Mejores Efectos Sonoros.",
                Genre = "Drama Belico", ReleaseDate = DateTime.Now.AddYears(-3),
                PosterUrl="https://www.clarin.com/img/2021/01/16/ij5Uc1CgY_1256x620__1.jpg",
                TrailerUrl="https://www.youtube.com/watch?v=Y3dH5bBt1sw&ab_channel=AntonioBenicioHuerga"},

                new Movie { Title = "LOS SIETE PECADOS CAPITALES", Rating = 7.2f,
                Description = "DescriUn enfermo psicópata con delirio místico se convierte en un asesino en serie que ejecuta a sus víctimas regido por los siete pecados capitales enunciados en la Biblia: gula, avaricia, pereza, lujuria, soberbia, envidia e ira. Enorme papel protagonizado por un ascendente Kevin Spacey en los años que también sacudiría a la Academia llevándose el Oscar protagónico por Belleza Americana. \n Ante la locura de este asesino, William Somerset (Morgan Freeman), teniente del departamento de homicidios y a punto de jubilarse, entra en acción acompañado por el joven detective David Mills (Brad Pitt). En una carrera contra el tiempo deberán atrapar al asesino para evitar que más horror se apodere de la sociedad.",
                Genre = "Suspenso Drama", ReleaseDate = DateTime.Now.AddYears(-4),
                PosterUrl="https://www.clarin.com/img/2021/01/16/CDZBStIMi_1256x620__1.jpg",
                TrailerUrl="https://www.youtube.com/watch?v=xhzBmjdehPA&ab_channel=TERRORLAND"},

                new Movie { Title = "VOLVER AL FUTURO", Rating = 7.2f,
                Description = "Un científico loco inventa la máquina del tiempo sobre la carrocería de un DeLorean, un auto deportivo de los años 80, e invita a su joven amigo Marty McFly para probar su efectividad. Pero la imprevista irrupción de un grupo terrorista que quiere hacerse del gran invento lleva a Marty a escapar en el DeLorean hacia el pasado. \n ​Su destino: los años 50, donde se encontrará con sus padres adolescentes que todavía ni se conocen. La interacción del joven con ambos pondrá en peligro su existencia en el presente. Una obra maestra que permanece como un clásico para los nuevos chicos y jóvenes que descubren el cine. Ganadora de un premio Oscar a la Mejor Edición de Sonido.",
                Genre = "Ciencia-Ficcion", ReleaseDate = DateTime.Now.AddYears(-3),
                PosterUrl="https://www.clarin.com/img/2020/10/20/D1EH94nMn_1256x620__1.jpg",
                TrailerUrl="https://www.youtube.com/watch?v=aGwLwNz7j34&ab_channel=Empeliculados.Co"},

                new Movie { Title = "GLADIADOR", Rating = 7.2f,
                Description = "Librada la gran victoria sobre los Bárbaros del norte del Imperio Romano en el año 180 DC., el emperador Marco Aurelio, agotado y enfermo, elige a su general más leal, Máximo, para continuar su legado. Esta noticia lleva a su hijo Cómodo, heredero directo al trono, a matar al emperador en su lecho mientras duerme y mandar a exterminar también a Máximo y su familia. \n Máximo logra escapar, pero es tomado prisionero por un traficante de esclavos que lo vende al dueño de un ludus de luchadores de la arena. El protagonista hará lo imposible por llegar al Coliseo Romano como gladiador estrella y enfrentar a su enemigo, Cómodo, para vengar a su familia asesinada por orden del autoproclamado emperador, en un duelo actoral que enfrenta a los enormes actores Russell Crowe y Joaquin Phoenix. \n ​Ganadora de cinco premios Oscar: Mejor Película, Mejor Actor protagónico (Russell Crowe), Mejor Diseño de Vestuario, Mejores Efectos Visuales y Mejor Sonido.",
                Genre = "Genre 4", ReleaseDate = DateTime.Now.AddYears(-4),
                PosterUrl="https://www.clarin.com/img/2021/01/16/IU8wKpbOH_1256x620__1.jpg",
                TrailerUrl="https://www.youtube.com/watch?v=P5ieIbInFpg&ab_channel=ParamountMovies"},

                new Movie { Title = "APOCALIPSIS NOW REDUX", Rating = 7.2f,
                Description = "Otra de las joyas del director Francis Ford Coppola nuevamente en colaboración con el actor Marlon Brando, quien siente años antes le había compuesto un inolvidable Don Corleone para El Padrino I. Han contado los protagonistas que Coppola tuvo nuevamente que luchar mucho para conseguir que Brando fuera a filmar a la selva, aún cuando cobraría la cifra increíble de 5 millones de dólares por 5 minutos de filmación. \n Pero todo el guión, y el atractivo de la película, estaba basado en más de dos horas de búsqueda en medio de la guerra para encontrar el perdido general norteamericano enloquecido que se había quedado armando su propia secta en la selva de Camboya. \n Un joven Martin Sheen hace del soldado que debe cruzar esa guerra enloquecida para enfrentarse e intentar matar al general místico que armó su propio ejército y a quienes los nativos adoran como a un rey o un Buda. El resultado es absolutamente genial: una de las más originales películas de guerra de la historia del cine. Ganadora de dos Oscars: Mejor Fotografía y Mejor Sonido.",
                Genre = "Drama Belico", ReleaseDate = DateTime.Now.AddYears(-5),
                PosterUrl="https://www.clarin.com/img/2021/01/16/rhdNmmjai_1256x620__1.jpg",
                TrailerUrl="https://www.youtube.com/watch?v=CxENJ2LwecY&ab_channel=ryy79"}
            };

            // Add movies to the context if they do not already exist
            foreach (var movie in movies)
            {
                if (!context.Movies.Any(m => m.Title == movie.Title))
                {
                    context.Movies.Add(movie);
                }
            }
            await context.SaveChangesAsync();

            // Fetch the saved movies from the context to get their IDs
            var savedMovies = context.Movies.ToList();

            // Seed movie subscriptions
            var movieSubscriptions = new List<MovieSubscription>
            {
                new MovieSubscription { MovieId = savedMovies.First(m => m.Title == "EL PADRINO I").Id, SubscriptionId = savedSubscriptions.First(s => s.Type == "Started").Id },
                new MovieSubscription { MovieId = savedMovies.First(m => m.Title == "BATMAN: EL CABALLERO DE LA NOCHE").Id, SubscriptionId = savedSubscriptions.First(s => s.Type == "Started").Id },
                new MovieSubscription { MovieId = savedMovies.First(m => m.Title == "LA LISTA DE SCHINDLER").Id, SubscriptionId = savedSubscriptions.First(s => s.Type == "Started").Id },
                new MovieSubscription { MovieId = savedMovies.First(m => m.Title == "FORREST GUMP").Id, SubscriptionId = savedSubscriptions.First(s => s.Type == "Started").Id },
                new MovieSubscription { MovieId = savedMovies.First(m => m.Title == "EL ORIGEN").Id, SubscriptionId = savedSubscriptions.First(s => s.Type == "Started").Id },

                new MovieSubscription { MovieId = savedMovies.First(m => m.Title == "EL PADRINO I").Id, SubscriptionId = savedSubscriptions.First(s => s.Type == "Premium").Id },
                new MovieSubscription { MovieId = savedMovies.First(m => m.Title == "BATMAN: EL CABALLERO DE LA NOCHE").Id, SubscriptionId = savedSubscriptions.First(s => s.Type == "Premium").Id },
                new MovieSubscription { MovieId = savedMovies.First(m => m.Title == "LA LISTA DE SCHINDLER").Id, SubscriptionId = savedSubscriptions.First(s => s.Type == "Premium").Id },
                new MovieSubscription { MovieId = savedMovies.First(m => m.Title == "FORREST GUMP").Id, SubscriptionId = savedSubscriptions.First(s => s.Type == "Premium").Id },
                new MovieSubscription { MovieId = savedMovies.First(m => m.Title == "EL ORIGEN").Id, SubscriptionId = savedSubscriptions.First(s => s.Type == "Premium").Id },
                new MovieSubscription { MovieId = savedMovies.First(m => m.Title == "MATRIX").Id, SubscriptionId = savedSubscriptions.First(s => s.Type == "Premium").Id },
                new MovieSubscription { MovieId = savedMovies.First(m => m.Title == "PARÁSITE").Id, SubscriptionId = savedSubscriptions.First(s => s.Type == "Premium").Id },
                new MovieSubscription { MovieId = savedMovies.First(m => m.Title == "RESCATANDO AL SOLDADO RYAN").Id, SubscriptionId = savedSubscriptions.First(s => s.Type == "Premium").Id },
                new MovieSubscription { MovieId = savedMovies.First(m => m.Title == "LOS SIETE PECADOS CAPITALES").Id, SubscriptionId = savedSubscriptions.First(s => s.Type == "Premium").Id },
                new MovieSubscription { MovieId = savedMovies.First(m => m.Title == "VOLVER AL FUTURO").Id, SubscriptionId = savedSubscriptions.First(s => s.Type == "Premium").Id },
                new MovieSubscription { MovieId = savedMovies.First(m => m.Title == "GLADIADOR").Id, SubscriptionId = savedSubscriptions.First(s => s.Type == "Premium").Id },
                new MovieSubscription { MovieId = savedMovies.First(m => m.Title == "APOCALIPSIS NOW REDUX").Id, SubscriptionId = savedSubscriptions.First(s => s.Type == "Premium").Id }
            };

            foreach (var movieSubscription in movieSubscriptions)
            {
                if (!context.MovieSubscription.Any(ms => ms.MovieId == movieSubscription.MovieId && ms.SubscriptionId == movieSubscription.SubscriptionId))
                {
                    context.MovieSubscription.Add(movieSubscription);
                }
            }
            await context.SaveChangesAsync();
        }
    }
}
