using System;
using System.Collections.Generic;
using System.Linq;
using VAA.DataAccess.Interfaces;
using VAA.DataAccess.Model;
using VAA.Entities.VAAEntity;

namespace VAA.DataAccess
{
    /// <summary>
    /// Base Item Management Class - Hanldes base item get, add, edit, delete
    /// </summary>
    public class BaseItemManagement : IBaseItem
    {
        private readonly VAAEntities _context = new VAAEntities();

        public List<BaseItem> GetBaseItems(int classId, int menuTypeId, int languageId)
        {
            try
            {
                var baseItemData = _context.sp_GetAll_tBaseItems(classId, menuTypeId, languageId).ToList();

                return (from data in baseItemData
                        select new BaseItem
                        {
                            BaseItemId = data.BaseItemID,
                            BaseItemCode = data.BaseItemCode,
                            ClassId = data.ClassID,
                            MenuTypeId = data.MenuTypeID,
                            LanguageId = data.LanguageId,
                            CategoryName = data.CategoryName,
                            CategoryId = data.CategoryID,
                            BaseItemTitle = data.BaseItemTitle,
                            BaseItemTitleDescription = data.BaseItemTitleDescription,
                            BaseItemDescription = data.BaseItemDescription,
                            BaseItemSubDescription = data.BaseItemSubDescription,
                            BaseItemAttributes = data.BaseItemAttributes
                        }).ToList();
            }
            catch (Exception ex)
            {
                return new List<BaseItem>();
            }
        }
        public List<BaseItem> GetBaseItemList(string baseItemCode)
        {
            try
            {
                return (from baseitem in _context.tBaseItems
                        join menucat in _context.tMenuItemCategory on baseitem.CategoryID equals menucat.ID
                        where baseitem.BaseItemCode == baseItemCode
                        select new BaseItem
                        {
                            BaseItemId = baseitem.ID,
                            BaseItemCode = baseitem.BaseItemCode,
                            ClassId = baseitem.ClassID,
                            MenuTypeId = baseitem.MenuTypeID,
                            LanguageId = baseitem.LanguageId,
                            CategoryName = menucat.CategoryName,
                            CategoryId = baseitem.CategoryID,
                            BaseItemTitle = baseitem.BaseItemTitle,
                            BaseItemTitleDescription = baseitem.BaseItemTitleDescription,
                            BaseItemDescription = baseitem.BaseItemDescription,
                            BaseItemSubDescription = baseitem.BaseItemSubDescription,
                            BaseItemAttributes = baseitem.BaseItemAttributes
                        }).ToList();
            }
            catch (Exception ex)
            {
                return new List<BaseItem>();
            }
        }

        public BaseItem GetBaseItem(long baseItemId)
        {
            try
            {
                var baseItemData = (from baseItem in _context.tBaseItems where baseItem.ID == baseItemId select baseItem).FirstOrDefault();

                if (baseItemData != null)
                {
                    var categoryId = baseItemData.CategoryID;

                    var category =
                        (from c in _context.tMenuItemCategory where c.ID == categoryId select c).FirstOrDefault();

                    if (category != null)
                        return new BaseItem()
                        {
                            BaseItemId = baseItemData.ID,
                            BaseItemCode = baseItemData.BaseItemCode,
                            ClassId = baseItemData.ClassID,
                            MenuTypeId = baseItemData.MenuTypeID,
                            CategoryId = baseItemData.CategoryID,
                            CategoryCode = category.CategoryCode,
                            CategoryName = category.CategoryName,
                            BaseItemTitle = baseItemData.BaseItemTitle,
                            BaseItemTitleDescription = baseItemData.BaseItemTitleDescription,
                            BaseItemDescription = baseItemData.BaseItemDescription,
                            BaseItemSubDescription = baseItemData.BaseItemSubDescription,
                            BaseItemAttributes = baseItemData.BaseItemAttributes
                        };

                    return new BaseItem() { BaseItemId = 0 };
                }
                return new BaseItem() { BaseItemId = 0 };
            }
            catch (Exception ex)
            {
                return new BaseItem() { BaseItemId = 0 };
            }
        }


        public BaseItem GetBaseItem(string baseItemCode)
        {
            try
            {
                var baseItemData = (from baseItem in _context.tBaseItems where baseItem.BaseItemCode == baseItemCode select baseItem).FirstOrDefault();

                if (baseItemData != null)
                {
                    var categoryId = baseItemData.CategoryID;

                    var category =
                        (from c in _context.tMenuItemCategory where c.ID == categoryId select c).FirstOrDefault();

                    if (category != null)
                        return new BaseItem()
                        {
                            BaseItemId = baseItemData.ID,
                            BaseItemCode = baseItemData.BaseItemCode,
                            ClassId = baseItemData.ClassID,
                            MenuTypeId = baseItemData.MenuTypeID,
                            CategoryId = baseItemData.CategoryID,
                            CategoryCode = category.CategoryCode,
                            CategoryName = category.CategoryName,
                            BaseItemTitle = baseItemData.BaseItemTitle,
                            BaseItemTitleDescription = baseItemData.BaseItemTitleDescription,
                            BaseItemDescription = baseItemData.BaseItemDescription,
                            BaseItemSubDescription = baseItemData.BaseItemSubDescription,
                            BaseItemAttributes = baseItemData.BaseItemAttributes
                        };

                    return null;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public long CreateNewBaseItem(BaseItem baseItem)
        {
            try
            {
                tBaseItems newBaseItem = new tBaseItems
                {
                    BaseItemCode = baseItem.BaseItemCode,
                    ClassID = baseItem.ClassId,
                    CategoryID = baseItem.CategoryId,
                    MenuTypeID = baseItem.MenuTypeId,
                    LanguageId = baseItem.LanguageId,
                    BaseItemTitle = baseItem.BaseItemTitle,
                    BaseItemTitleDescription = baseItem.BaseItemTitleDescription,
                    BaseItemDescription = baseItem.BaseItemDescription,
                    BaseItemSubDescription = baseItem.BaseItemSubDescription,
                    BaseItemAttributes = baseItem.BaseItemAttributes
                };
                _context.tBaseItems.Add(newBaseItem);
                _context.SaveChanges();
                return newBaseItem.ID;

            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public bool DeleteBaseItem(long baseItemId)
        {
            try
            {
                var baseItemToDelete = (from tBaseItems in _context.tBaseItems where tBaseItems.ID == baseItemId select tBaseItems).FirstOrDefault();
                if (baseItemToDelete != null)
                {
                    _context.tBaseItems.Remove(baseItemToDelete);
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool UpdateBaseItem(BaseItem baseItem)
        {
            try
            {
                var baseItemUpdate = (from tBaseItems in _context.tBaseItems where tBaseItems.ID == baseItem.BaseItemId select tBaseItems).FirstOrDefault();

                if (baseItemUpdate != null)
                {
                    baseItemUpdate.BaseItemCode = baseItem.BaseItemCode;
                    baseItemUpdate.CategoryID = baseItem.CategoryId;
                    baseItemUpdate.BaseItemTitle = baseItem.BaseItemTitle;
                    baseItemUpdate.BaseItemTitleDescription = baseItem.BaseItemTitleDescription;
                    baseItemUpdate.BaseItemDescription = baseItem.BaseItemDescription;
                    baseItemUpdate.BaseItemSubDescription = baseItem.BaseItemSubDescription;
                    baseItemUpdate.BaseItemAttributes = baseItem.BaseItemAttributes;
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool ExistingBaseItem(string baseItemCode)
        {
            try
            {
                var baseItemToCheck = (from tBaseItems in _context.tBaseItems where tBaseItems.BaseItemCode == baseItemCode select tBaseItems).FirstOrDefault();
                if (baseItemToCheck != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public List<tMenuLanguage> GetAllLanguages()
        {
            return _context.tMenuLanguage.ToList();
        }

    }
}
