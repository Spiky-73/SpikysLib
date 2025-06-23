# Spiky's Lib
Just a small library used by most of my mods.

## Content
- Custom UI elements
- Terraria constants
- Custom data structures
- Extension methods from Terraria classes
- Helper methods
- A generic reflection wrapper
- Notification templates
- Generic Math
- Config Porting tools
- An api for custom cursors

# Changelog

### v1.3.1.2
- Ported to MagicStorage v0.7
- Added a custom StorageAggregator to MagicStorage
- Disabled MagicStorageIntegration.StackFix

### v1.3.1.1
- Fixed a complete freeze of the of game when joining multiplayer
- Fixed UIFlex elements incorrect size on Recalculate

### v1.3.1
- Added GraphicsHelper.DrawTexture
- Added a workaround to allow items to stack inside Magic Storage

### v1.3
- Added a GUID to items
- Added DrawStringWithShadow and DrawStringShadow methods to GraphicsHelper
- Added IsPartOfACurrency, CurrencyValue and CurrencyType method for item types to CurrencyHelper
- Added ItemHelper.IsInventoryContext
- Added GraphicsHelper.DrawMouseText
- Moved HoverImage and HoverImageSplit to SpikysLib.UI.Elements
- Added SpikysLib.UI.Elements.HoverImageFramed
- Added SpikysLib.UI.Elements.UIFlexGrid and UIFlexList
- Improved CurrencyHelper.GetPriceText method
- Changed MagicStorageIntegration method signatures
- Fixed HoverImage tooltip not been drawn in-game
- Fixed TooltipHelper.AddLine not always adding the line
- Fixed PlayerHelper.CountItems not working wing Quality of Terraria

### v1.2.0.1
- Fixed a crash when using ObjectMemberElement with null values

### v1.2
- Added Expand/Collapse button to EntityDefinitionElement
- Added LanguageHelper class
- Added IPreLoadMod interface
- Added ObjectMembersElement class
- Added ObjectElement class
- Added methods to CollectionHelper
- Added OrderedDictionary\<TKey, TValue>
- Added ModPacketHandler and PacketHandlerLoader
- Merged ModConfigExtensions and PortConfig into ConfigHelper
- Merged Graphics and GraphicsExtensions into GraphicsHelper
- Renamed Extensions classes to Helper
- Removed obsolete v1.0 items
- Removed conditions on NestedValue
- Fixed a bug with ValueWrappers without generic args
- Fixed a bug with NestedValue serialization
- Fixed CountItems bug when called serverside
- Fixed typos
- Fixed tooltips issues
- Fixed a bug with WrapperStringConverter

### v1.1.0.1
- Fixed serialization bugs

### v1.1
- Updated icon
- Added class DictionaryValuesElement and ValueWrapperAttribute
- Added class EntityDefinition\<T>
- Renamed property Parent to Key in NestedValue
- Added MoveMember to ModConfigExtensions
- Renamed Index to ItemAt in CollectionExtensions
- Added method FindIndex to CollectionExtensions
- Added methods GotoLoc and FindLoc to ILExtensions
- Added method GetPropertiesFields to TypeExtensions
- Added hover tooltip to InGameNotification
- Moved ReflectionHelper members to TypeExtensions
- Renamed method GetMember to GetPropertyFieldValue in TypeExtensions
- Removed methods GetMember, GetField and GetProperty from ReflectionHelper
- Replaced Joined\<TList, T> by JoinedLists\<T>
- Implemented new IList\<T> methods in ListIndices\<T>
- Replaced Cache\<TKey, TValue> by GeneratedDictionary\<TKey, TValue>
- Added CursorLoader class
- Added class PortMember
- Added ItemSlots constants
- Added class ToFromStringConverter
- Improved Json Serialization
- Loosened NestedValues conditions
- Fixed a bug with MultiChoice tooltip
- Fixed MultiChoice swap button labels
- Fixed bugs with Range and RangeSet

### v1.0.1
- Fixed mod not loading on the server
- Made MathX generic
- Added MathX.Snap

### v1.0
- Initial release on tML