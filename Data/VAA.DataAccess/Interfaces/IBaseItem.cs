using System.Collections.Generic;
using VAA.DataAccess.Model;
using VAA.Entities.VAAEntity;

namespace VAA.DataAccess.Interfaces
{
    /// <summary>
    /// Base Item related operations
    /// </summary>
    public interface IBaseItem
    {
        List<BaseItem> GetBaseItems(int classId, int menuTypeId, int languageId);
        List<BaseItem> GetBaseItemList(string baseItemCode);
        BaseItem GetBaseItem(long baseItemId);
        BaseItem GetBaseItem(string baseItemCode);
        long CreateNewBaseItem(BaseItem baseItem);
        bool DeleteBaseItem(long baseItemId);
        bool UpdateBaseItem(BaseItem baseItem);
        bool ExistingBaseItem(string baseItemCode);
        List<tMenuLanguage> GetAllLanguages();
    }
}
