﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using TicketLand_project.Models;
using TicketLand_project.ViewModels;

namespace TicketLand_project.Controllers
{

    public class HomeController : Controller
    {

        QUANLYXEMPHIMEntities objModel = new QUANLYXEMPHIMEntities();
        public async Task<ActionResult> Index()
        {
            if (Session["Username"] != null)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://apitikketland.azurewebsites.net/api/");
                    HttpResponseMessage response = await client.GetAsync("movies");

                    if (response.IsSuccessStatusCode)
                    {
                        var movies = await response.Content.ReadAsAsync<List<movy>>();
                        foreach (movy movie in movies)
                        {
                            movie.DurationInMinutes = ConvertTimeToMinutes(movie.movie_duration.ToString());
                        }
                        return View(movies);
                    }
                    else
                    {
                        // Xử lý khi không nhận được phản hồi thành công từ API
                        // Ví dụ: ghi log, hiển thị thông báo lỗi, vv.
                        return View("Error");
                    }
                }
            }

            else
            {
                return View();
            }

        }

        public static int ConvertTimeToMinutes(string time)
        {
            TimeSpan timeSpan;
            if (TimeSpan.TryParse(time, out timeSpan))
            {
                int minutes = timeSpan.Hours * 60 + timeSpan.Minutes;
                return minutes;
            }

            // Nếu định dạng thời gian không hợp lệ hoặc không thể chuyển đổi thành TimeSpan, trả về giá trị mặc định hoặc ném ra một ngoại lệ tùy thuộc vào yêu cầu của bạn.
            return 0; // Giá trị mặc định (hoặc bạn có thể trả về một giá trị khác)
        }

        // GET: Register
        public ActionResult Register()
        {
            return View();
        }


        public class News
        {
            public int news_id { get; set; }
            public int movie_id { get; set; }
            public string news_title { get; set; }
            public string news_content { get; set; }
            public string news_img { get; set; }
            public DateTime news_release { get; set; }
        }



        public async Task<ActionResult> GetMovieTimes(string movieName, DateTime scheduleDate)
        {
            if (scheduleDate != null)
            {
                // Gọi API và lấy dữ liệu trả về
                var apiUrl = "https://apitikketland.azurewebsites.net/api/schedules";
                var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(apiUrl);
                var responseData = await response.Content.ReadAsStringAsync();

                var apiUrl_2 = "https://apitikketland.azurewebsites.net/api/movies";
                var httpClient2 = new HttpClient();
                var response2 = await httpClient2.GetAsync(apiUrl_2);
                var responseData2 = await response2.Content.ReadAsStringAsync();

                var movies = Newtonsoft.Json.JsonConvert.DeserializeObject<List<movy>>(responseData2);
                // Chuyển đổi dữ liệu từ JSON sang danh sách lịch chiếu
                var schedules = Newtonsoft.Json.JsonConvert.DeserializeObject<List<schedule>>(responseData);
                Debug.WriteLine("movies: " + movieName);

                var filterMovies = movies.Where(m => m.movie_name == movieName);
                Debug.WriteLine("movies: " + filterMovies);
                int movie_id = filterMovies.Select(m => m.movie_id).FirstOrDefault();

                Debug.WriteLine("movies: " + movie_id);
                // Lọc danh sách lịch chiếu theo movie_id và schedule_date
                var filteredSchedules = schedules.Where(s => s.movie_id == movie_id && s.show_date == scheduleDate);

                // Lấy danh sách các time_start
                var movieTimes = filteredSchedules.Select(s => s.time_start).ToList();

                // In danh sách time_start ra Output Debug
                foreach (var time in movieTimes)
                {
                    Debug.WriteLine("time_start: " + time);
                }

                return Json(movieTimes, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Debug.WriteLine("Không có lịch chiếu");
                return View();
            }
        }


        public async Task<ActionResult> GetNews()
        {
            // Gọi API và lấy dữ liệu trả về
            var apiUrl = "https://apitikketland.azurewebsites.net/api/news";
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(apiUrl);
            var responseData = await response.Content.ReadAsStringAsync();

            // Chuyển đổi dữ liệu từ chuỗi JSON sang đối tượng C#
            var newsList = JsonConvert.DeserializeObject<List<News>>(responseData);

            // Lọc ra những news có news_release cách ngày hiện tại không quá 10 ngày
            var filteredNewsList = new List<News>();

            foreach (var news in newsList)
            {
                DateTime now = DateTime.Now;
                DateTime newsRelease = news.news_release; // hoặc news.news_release.GetValueOrDefault()
                TimeSpan timeDiff = now - newsRelease;

                double daysDiff = timeDiff.TotalDays;
                if (daysDiff <= 10)
                {
                    filteredNewsList.Add(news);
                }
            }

            return Json(filteredNewsList, JsonRequestBehavior.AllowGet);
        }


        public static string GenerateSlug(string title)
        {
            string slug = Regex.Replace(title, @"[^a-zA-Z0-9-]", "-");
            slug = Regex.Replace(slug, @"-{2,}", "-");
            slug = slug.Trim('-').ToLower();
            return slug;
        }

        // Chức năng đăng kí
        //POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(member _user, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                var check = objModel.members.FirstOrDefault(s => s.username == _user.username);
                if (check == null)
                {
                    //Mã hóa mật khẩu
                    _user.password = GetMD5(_user.password);

                    if (imageFile != null && imageFile.ContentLength > 0)
                    {
                        var slug = GenerateSlug(_user.member_name);
                        var memberFileName = Path.GetFileName(imageFile.FileName);
                        var extension = Path.GetExtension(memberFileName);
                        var new_file_name = $"{slug}{extension}";
                        var relativePosterPath = "\\Assets\\img\\home\\avatar_member\\" + new_file_name;
                        var absolutePosterPath = Path.Combine(Server.MapPath("~/Assets/img/home/avatar_member/"), new_file_name);

                        _user.member_avatar = relativePosterPath;

                        // Lưu file vào máy chủ với tên mới
                        using (var fileStream = new FileStream(absolutePosterPath, FileMode.Create))
                        {
                            imageFile.InputStream.Seek(0, SeekOrigin.Begin);
                            imageFile.InputStream.CopyTo(fileStream);
                        }
                    }
                    _user.role_id = 2;
                    _user.member_point = 0;
                    objModel.Configuration.ValidateOnSaveEnabled = false;
                    objModel.members.Add(_user);

                    objModel.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "Tên người dùng đã tồn tại";
                }

            }
            return View();
        }

        //Tạo MD5
        public static string GetMD5(string str)
        {
            string byte2String = null;
            if (str != null)
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] fromData = Encoding.UTF8.GetBytes(str);
                byte[] targetData = md5.ComputeHash(fromData);

                for (int i = 0; i < targetData.Length; i++)
                {
                    byte2String += targetData[i].ToString("x2");

                }
            }
            return byte2String;
        }


        //Chức năng đăng nhập
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            if (ModelState.IsValid)
            {

                if (username != "" && password != "")
                {
                    var f_password = GetMD5(password);
                    var data = objModel.members.Where(s => s.username.Equals(username) && s.password.Equals(f_password)).ToList();
                    if (data.Count() > 0)
                    {
                        //add session
                        Session["Username"] = data.FirstOrDefault().username;
                        Session["idMember"] = data.FirstOrDefault().member_id.ToString();
                        Session["IsLoggedIn"] = "1";
                        if (string.IsNullOrEmpty(data.FirstOrDefault().member_avatar?.ToString()))
                        {
                            Session["Avartar"] = null;
                        }
                        else
                        {
                            Session["Avartar"] = data.FirstOrDefault().member_avatar.ToString();
                        }
                        // 2: user, 1: admin
                        if (data.FirstOrDefault().role_id == 2)
                        {
                            return RedirectToAction("Index");
                        }
                        else if (data.FirstOrDefault().role_id == 1)
                        {
                            return RedirectToAction("Index", "manage_members", new { area = "Admin" });
                        }
                    }
                    else if (data.Count() == 0)
                    {
                        ViewBag.Message = "Tài khoản không hợp lệ";
                    }
                }
                else
                {
                    ViewBag.Message = "Vui lòng nhập thông tin tài khoản";
                }
            }
            return View();
        }

        //Lấy thông tin session để hiển thị ra session storage
        public JsonResult GetUserInfo()
        {
            string username = Session["Username"] as string ?? "Guest";
            string idMember = Session["idMember"] as string ?? "-1";
            string isLogin = Session["IsLoggedIn"] as string ?? "0";
            string avatar = Session["Avartar"] as string ?? "Null";
            return Json(new { Username = username, IdMember = idMember, IsLogin = isLogin, Avatar = avatar }, JsonRequestBehavior.AllowGet);
        }




        //Logout
        public ActionResult Logout()
        {
            Session.Clear();//remove session
            return RedirectToAction("Login");
        }

 
        public ActionResult ScrollToPosition()
        {
            return RedirectToAction("Index");
        }


        public ActionResult MovieDetail(int id)
        {
            ViewBag.Message = "Movie Detail";

            //string decodedTitle = System.Web.HttpUtility.UrlDecode(title);
            var movieEntity = objModel.movies.FirstOrDefault(m => m.movie_id == id);
            if (movieEntity != null)
            {
                // Chuyển đổi từ Entity sang ViewModel
                var viewModel = new MovieDetailViewModel
                {
                    Id = movieEntity.movie_id,
                    Title = movieEntity.movie_name,
                    Genre = movieEntity.movie_genres,
                    Director = movieEntity.movie_director,
                    Actors = movieEntity.movie_actor,
                    ReleaseDate = (DateTime)movieEntity.movie_release,
                    Description = movieEntity.movie_description,
                    PosterUrl = movieEntity.movie_poster,
                    Trailer = GetDataAfterString(movieEntity.movie_trailer, ".be/"),
                    Duration = movieEntity.movie_duration,
                    Format = movieEntity.movie_format,
                    MovieCens = movieEntity.movie_cens,
                    Comments = movieEntity.comments.Select(c => new CommentViewModel
                    {
                        CommentId = c.comment_id,
                        Content = c.content,
                        CommentStar = (float)c.comment_star,
                        CommentDate = (DateTime)c.comment_date,
                        MemberName = c.member.member_name,
                        MemberAvatar = c.member.member_avatar,
                    }).Reverse().ToList(),
                    AverageRating = /*(float)movieEntity.rate,*/
                   (float)Math.Round((double)(movieEntity.comments.Any() ? movieEntity.comments.Average(c => c.comment_star) : 0), 1),

                    Schedules = objModel.schedules.Where(s => s.movie_id == movieEntity.movie_id).ToList(),
                    Rooms = objModel.rooms.ToList()
                };
                // Thêm các thông tin chi tiết khác của phim

                return View(viewModel);
            }
            else
            {
                return HttpNotFound();
            }

            // Truyền dữ liệu vào view
        }
        [HttpPost]
        public ActionResult AddComment(int movieId, string content, float commentStar)
        {
            // Lấy thông tin người dùng hiện tại (đã đăng nhập)
            var currentUserId = GetCurrentUserId();

            if (currentUserId == -1)
            {
                // Lưu đường dẫn trước đó vào session để sử dụng sau khi đăng nhập
                Session["ReturnUrlAfterLogin"] = Request.UrlReferrer?.ToString();
                // Xử lý khi người dùng chưa đăng nhập
                return RedirectToAction("Login");
            }

            // Tạo đối tượng Comment
            var newComment = new comment
            {
                movie_id = movieId,
                member_id = currentUserId,
                content = content,
                comment_star = commentStar,
                comment_date = DateTime.Now,
            };
            // Thêm đánh giá và bình luận mới vào cơ sở dữ liệu
            objModel.comments.Add(newComment);
            objModel.SaveChanges();

            // Lấy thông tin phim và cập nhật AverageRating
            var movie = objModel.movies.Find(movieId);

            if (movie != null)
            {
                // Tính toán lại rating dựa trên các đánh giá
                movie.rate = (float)Math.Round((double)(movie.comments.Any() ? movie.comments.Average(c => c.comment_star) : 0), 1);

                // Cập nhật bản ghi trong cơ sở dữ liệu
                objModel.SaveChanges();
            }
            int id = movieId;

            // Chuyển hướng về trang chi tiết phim
            return RedirectToAction("MovieDetail", new { id });
        }
        private string GetDataAfterString(string input, string substring)
        {
            // Kiểm tra xem chuỗi có chứa dấu '=' không
            int index = input.IndexOf(substring);

            if (index != -1)
            {
                // Lấy các ký tự sau chuỗi
                string dataAfterSubstring = input.Substring(index + substring.Length);
                return dataAfterSubstring;
            }

            // Trả về null nếu không tìm thấy trong chuỗi
            return null;
        }
        private int GetCurrentUserId()
        {
            // Lấy ID từ session
            if (Session["idMember"] != null)
            {
                // Ép kiểu Session["idMember"] về kiểu int
                if (int.TryParse(Session["idMember"].ToString(), out int memberId))
                {
                    Debug.WriteLine($"Current User ID: {memberId}");
                    // Trả về memberId
                    return memberId;
                }
            }

            Debug.WriteLine("Session 'idMember' not found or cannot be parsed as int.");
            return 0;
        }
        [HttpGet]
        public JsonResult GetDates(int roomNumber, int movieId)
        {
            // Lấy danh sách các ngày chiếu từ cơ sở dữ liệu dựa trên phòng
            //var dateString = objModel.schedule.Value.ToString("yyyy-MM-dd");
            var dates = objModel.schedules
                .Where(s => s.movie_id == movieId && s.room_id == roomNumber)
                .Select(s => s.show_date)
                .Distinct()
                .ToList();
            var formattedDates = dates.Select(d => d.ToString("yyyy-MM-dd")).ToList();
            return Json(formattedDates, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetShowtimes(int roomNumber, DateTime date, int movieId)
        {
            var targetDate = date.Date;
            // Lấy danh sách thời gian chiếu từ cơ sở dữ liệu dựa trên phòng, phim và ngày
            var rawShowtimes = objModel.schedules
                .Where(s => s.movie_id == movieId && s.room_id == roomNumber && DbFunctions.TruncateTime(s.show_date) == targetDate)
                .Select(s => new { StartTime = s.time_start, EndTime = s.time_end, ScheduleId = s.schedule_id })
                .ToList();

            var showtimes = rawShowtimes
             .Where(s => s.StartTime != null && s.EndTime != null)
             .Select(s => new
             {
                 ScheduleId = s.ScheduleId,
                 StartTime = ((TimeSpan)s.StartTime).ToString(@"hh\:mm"),
                 EndTime = ((TimeSpan)s.EndTime).ToString(@"hh\:mm")
             })
             .ToList();

            return Json(showtimes, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SelectSeat(int scheduleId)
        {
            var schedule = objModel.schedules.FirstOrDefault(s => s.schedule_id == scheduleId);

            if (schedule == null)
            {
                return HttpNotFound();
            }

            // Lấy danh sách ghế dựa trên room_id
            var seats = objModel.seats.Where(s => s.room_id == schedule.room_id).ToList();

            // Truyền thông tin lịch chiếu và danh sách ghế vào view
            var viewModel = new SeatViewModel
            {
                Schedule = schedule,
                Seats = seats
            };

            return View(viewModel);
        }
    }
}
