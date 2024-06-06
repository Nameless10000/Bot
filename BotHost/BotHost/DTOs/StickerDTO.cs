using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace BotHost.DTOs
{
    public enum StickerType
    {
        NotCommand,
        BadCommand,
        NononoMisterFish,
        Hello,
        WorkInProgress
    }

    public class StickerDTO
    {
        private static readonly Dictionary<StickerType, InputFile> _stickers = new Dictionary<StickerType, InputFile>
    {
        { StickerType.NotCommand, InputFile.FromString("CAACAgIAAxkBAAEF7QNmYI5H6op8eacWz-5U5QOSnNlB-QACsToAAg-IcEqRrCbJQ4Uv9TUE") },
        { StickerType.BadCommand, InputFile.FromString("CAACAgIAAxkBAAEF7QlmYI_dRm2j14b-2m1vRgOFKaBwxwAC4j0AAl-PaUqyeSmkhxNJCDUE") },
        { StickerType.NononoMisterFish, InputFile.FromString("CAACAgIAAxkBAAEF7RtmYJKd7w_ZLJCbyjjSu1HM9HNFEgACbDcAAhBMkEpX9H_SMikrKjUE") },
        { StickerType.Hello, InputFile.FromString("CAACAgIAAxkBAAEF7aZmYK-BDiV_pQahDj5OxOIlJRQulQACxjAAAluPkEqow5iNYORvuTUE") },
        { StickerType.WorkInProgress, InputFile.FromString("CAACAgIAAxkBAAEF7cBmYLMUFbjj-qDvJoOVzzxXOIDrJQAChEMAAp4daEqVzKX5VFmspjUE") }
    };

        public static InputFile GetSticker(StickerType stickerType)
        {
            return _stickers.TryGetValue(stickerType, out var sticker) ? sticker : null;
        }
    }
}
