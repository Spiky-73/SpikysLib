# Spiky's Lib
Just a small library used by most of my mods.

## Content
- Custom UI elements
- Terraria constants
- Custom data structures
- Extension methods from Terraria classes
- A generic reflection wrapper
- Notification templates
- Generic Math
- Config Porting tools
- An api for custom cursors

# Changelog

### v1.2
- Merged ModConfigExtensions and PortConfig into ConfigHelper
- Merged Graphics and GraphicsExtensions into GraphicsHelper
- Renamed Extensions classes to Helper
- Removed obsolete v1.0 items

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