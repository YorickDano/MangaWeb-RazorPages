namespace MangaWeb.Managers
{
    public class UIValuesManager
    {
        public string Title { get; private set; } public string Manga { get; private set; }
        public string Users { get; private set; } public string Create { get; private set; }
        public string Hello { get; private set; } public string Register { get; private set; }
        public string Login { get; private set; } public string Logout { get; private set; }
        public string Score { get; private set; } public string Popularity { get; private set; }
        public string Ranked { get; private set; } public string Volumes { get; private set; } 
        public string Chapters { get; private set; } public string Published { get; private set; }
        public string Description { get; private set; }  public string Genres { get; private set; }
        public string Characters { get; private set; } public string Autors { get; private set; }
        public string AddToFavorite { get; private set; } public string Edit { get; private set; }
        public string Delete { get; private set; } public string Download { get; private set; } 
        public string UpdateInfo { get; private set; } public string CreateManually { get; private set; }
        public string CreateAuto { get; private set; } public string ManageAcc { get; private set; }
        public string ChangeYourAcc { get; private set; } public string Profile { get; private set; }
        public string Username { get; private set; } public string Password { get; private set; }
        public string ChangePassword { get; private set; } public string CurrentPassword { get; private set; }
        public string NewPassword { get; private set; } public string ConfNewPassword { get; private set; }
        public string Email { get; private set; } public string NewEmail { get; private set; }
        public string ChangeEmail { get; private set; } public string ManageEmail { get; private set; }
        public string Save { get; private set; } public string AddImage { get; private set; }
        public string SetRandAnimImg { get; private set; } public string PersData { get; private set; }
        public string PersDataDesc { get; private set; } public string PersDataDelDesc { get; private set; }
        public string ChooseLanguage { get; private set; } public string OrderBy { get; private set; }
        public string Year { get; private set; } public string From { get; set; } public string To { get; private set; }
        public string Submit { get; private set; } public string NoOptions { get; private set; }
        public string Image { get; private set; } public string Type { get; private set; }
        public string Status { get; private set; } public string Name { get; private set; }
        public string IsMain { get; private set; } public string EnterMangaTitle { get; private set; }
        public string YouWantRuManga { get; private set; }  public string SeeAll { get; private set; }
        private bool IsEnglish;
        public UIValuesManager()
        {
            SetEnglish();
            IsEnglish = true;
        }

        public void SetRussian()
        {
            IsEnglish = false; Title = "Название"; OrderBy = "Упорядочить по"; Year = "Год"; From = "От";
            To = "До"; Submit = "Подтвердить"; NoOptions = "Не выбрано"; Image = "Изображение";
            Manga = "Манга"; Users = "Пользователи"; Create = "Создать"; Hello = "Привет"; Logout = "Выйти";
            Score = "Оценка"; Popularity = "Популярность"; Ranked = "Ранг"; Volumes = "Тома";
            Chapters = "Главы"; Published = "Опубликовано"; Description = "Описание";
            Genres = "Жанры"; Characters = "Персонажи"; Autors = "Авторы"; AddToFavorite = "Добавить в любимое";
            Edit = "Редактировать"; Delete = "Удалить"; Download = "Скачать"; UpdateInfo = "Обновить информацию";
            CreateManually = "Создать мангу вручную"; CreateAuto = "Создать мангу автоматически"; 
            ManageAcc = "Управление учетной записью"; ChangeYourAcc = "Измените настройки своей учетной записи"; 
            Profile = "Профиль"; Username = "Имя пользователя"; Password = "Пароль"; 
            ChangePassword = "Изменить пароль"; CurrentPassword = "Действующий пароль";
            NewPassword = "Новый пароль"; ConfNewPassword = "Подтвердите новый пароль"; Email = "Почта";
            NewEmail = "Новая почта"; ChangeEmail = "Поменять почту"; ManageEmail = "Управление Почтой";
            Save = "Сохранить"; AddImage = "Добавить изображение"; SetRandAnimImg = "Установить случайное аниме изображение";
            PersData = "Персональная информация"; Type = "Тип"; Status = "Статус";
            PersDataDesc = "Ваша учетная запись содержит персональные данные, которые вы нам предоставили. Эта страница позволяет вам загружать или удалять эти данные.";
            PersDataDelDesc = "Удаление этих данных навсегда удалит вашу учетную запись, и это невозможно восстановить.";
            Register = "Зарегистрироваться"; Login = "Войти"; ChooseLanguage = "Выберите язык"; Name = "Имя";
            IsMain = "Главный?"; EnterMangaTitle = "Введите название манги"; YouWantRuManga = "Хотите мангу на русском?";
            SeeAll = "Все";
        } 

        public void SetEnglish()
        {
            IsEnglish = true; Title = "Title"; Title = "Title"; OrderBy = "Order by"; Year = "Year"; From = "From";
            To = "To"; Submit = "Submit"; NoOptions = "No options"; Image = "Image";
            Manga = "Manga"; Users = "Users"; Create = "Create"; Hello = "Hello"; Logout = "Logout";
            Score = "Score"; Popularity = "Popularity"; Ranked = "Ranked"; Volumes = "Volumes";
            Chapters = "Chapters"; Published = "Published"; Description = "Description";
            Genres = "Genres"; Characters = "Characters"; Autors = "Autors"; AddToFavorite = "Add to favorite";
            Edit = "Edit"; Delete = "Delete"; Download = "Download"; UpdateInfo = "Update info";
            CreateManually = "Create manga manually"; CreateAuto = "Auto manga creation"; ManageAcc = "Manage your account";
            ChangeYourAcc = "Change your account settings"; Profile = "Profile"; Username = "Username";
            Password = "Password"; ChangePassword = "Change password"; CurrentPassword = "Current password";
            NewPassword = "New password"; ConfNewPassword = "Confirm new password"; Email = "Email";
            NewEmail = "New email"; ChangeEmail = "Change email"; ManageEmail = "Manage Email";
            Save = "Save"; AddImage = "Add image"; SetRandAnimImg = "Set random anime profile image";
            PersData = "Personal data"; Type = "Type"; Status = "Status";
            PersDataDesc = "Your account contains personal data that you have given us. This page allows you to download or delete that data.";
            PersDataDelDesc = "Deleting this data will permanently remove your account, and this cannot be recovered.";
            Register = "Register"; Login = "Login"; ChooseLanguage = "Choose Language"; Name= "Name";
            IsMain = "Is main?"; EnterMangaTitle = "Enter title of manga"; YouWantRuManga = "You want manga on russian?";
            SeeAll = "See All";
        }

        public bool IsEnlish() => IsEnglish;
    }
}
