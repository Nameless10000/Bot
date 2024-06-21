using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace BotHost
{
    public enum StickerType
    {
        NotCommand,
        BadCommand,
        NononoMisterFish,
        Hello,
        WorkInProgress,
        RegistrationSuccess,
        RegistrationFailure,
        Disciplines,
        AppointmentSuccess,
        AppointmentFailure,
        GotAppointmentsAny,
        GotAppointmentsNo,
        NoTimeToAppoint,
        HasTimeToAppoint,
        AppointmentCancelSuccess,
        AppointmentCancelError,
        NOTUSED
    }

    // DTO - это объект для передачи данных (например, через HTTP REST запрос). Это скорее фабрика стикеров - StickerFactory
    // Так же, класс, экземпляр которого не должна быть возможность создать, должен быть помечен как static (У таких классов нельзя вызвать конструктор через new()).
    // Сама идея - топ
    public static class StickerFactory
    {
        private static readonly Dictionary<StickerType, InputFile> _stickers = new Dictionary<StickerType, InputFile>
        {
            { StickerType.NotCommand, InputFile.FromString("CAACAgIAAxkBAAEF7QNmYI5H6op8eacWz-5U5QOSnNlB-QACsToAAg-IcEqRrCbJQ4Uv9TUE") },
            { StickerType.BadCommand, InputFile.FromString("CAACAgIAAxkBAAEF7QlmYI_dRm2j14b-2m1vRgOFKaBwxwAC4j0AAl-PaUqyeSmkhxNJCDUE") },
            { StickerType.NononoMisterFish, InputFile.FromString("CAACAgIAAxkBAAEF7RtmYJKd7w_ZLJCbyjjSu1HM9HNFEgACbDcAAhBMkEpX9H_SMikrKjUE") },
            { StickerType.Hello, InputFile.FromString("CAACAgIAAxkBAAEF7aZmYK-BDiV_pQahDj5OxOIlJRQulQACxjAAAluPkEqow5iNYORvuTUE") },
            { StickerType.WorkInProgress, InputFile.FromString("CAACAgIAAxkBAAEF7cBmYLMUFbjj-qDvJoOVzzxXOIDrJQAChEMAAp4daEqVzKX5VFmspjUE") },
            { StickerType.RegistrationSuccess, InputFile.FromString("CAACAgIAAxkBAAEGAwVmZXgUk3Ix3HyerX61J2K3kB0vkAACLSsAAlpueUv_4I-yyhkbAjUE") },
            { StickerType.RegistrationFailure, InputFile.FromString("CAACAgIAAxkBAAEGAwFmZXdwqJwQ5Ht2KtNT_px5hRZ7-QACsj4AAlPoCEsYksjYAAGsptM1BA") },
            { StickerType.Disciplines, InputFile.FromString("CAACAgIAAxkBAAEGBGFmZa_XIdH-lwGkm4kYzApaD92LYgACvDUAAhMOaEhZwyURE0Hb9DUE") },
            { StickerType.AppointmentSuccess, InputFile.FromString("CAACAgIAAxkBAAEGBF9mZa8o8nBKRxBuXwlc1DLs0r79mAACizgAApEYmUqTb5rpDbZY-jUE") },
            { StickerType.AppointmentFailure, InputFile.FromString("CAACAgIAAxkBAAEGBFtmZa5rElnocmburDxRj_x_6zuJBAACnTkAAsAz6EpnHr2aMY8UcjUE") },
            { StickerType.GotAppointmentsAny, InputFile.FromString("CAACAgIAAxkBAAEGC6hmZy1Ip_vpcPi5ezsxl5iGujD5HQACjEEAAn33EUvaxR7N4KxNVDUE") },
            { StickerType.GotAppointmentsNo, InputFile.FromString("CAACAgIAAxkBAAEGC6JmZy0WM7Nw2QKokGkjNm0AAYw4hrUAAgI1AAIzB5FK_6OfOGROi6Y1BA") },
            { StickerType.NoTimeToAppoint, InputFile.FromString("CAACAgIAAxkBAAEGC8ZmZzi7etT36Inq7kWSZSfEpbV7AgACXioAAiiXgEo0hSj91_ul7zUE")},
            { StickerType.HasTimeToAppoint, InputFile.FromString("CAACAgIAAxkBAAEGDFxmZzmX0cvwBcrglFhI246KLI36fwAC7DAAAswVeUs2-fDhrl2n9DUE")},
            { StickerType.AppointmentCancelSuccess, InputFile.FromString("CAACAgIAAxkBAAEGVJBmdaCGuIzPT8yOcxAD1NLz0JgvXQACLzoAAkfukEo9_LWqutdZ2jUE") },
            { StickerType.AppointmentCancelError, InputFile.FromString("CAACAgIAAxkBAAEGVJJmdaD-bkvTEkWES0wN18sCjGTETQACNjUAArPV6UrxyxaeuuPw7zUE") },
            { StickerType.NOTUSED, InputFile.FromString("CAACAgIAAxkBAAEGBF1mZa66HI4AAfcdePi0_t7kJSQlWkQAAg8rAALTT3lKxOjTEIbMnhc1BA") }
        };

        public static InputFile GetSticker(StickerType stickerType)
        {
            return _stickers.TryGetValue(stickerType, out var sticker) ? sticker : null;
        }
    }
}
