# Spiky's Lib
Just a small library used by most of my mods.

## Content
- Custom UI elements
- Terraria contants
- Custom data structures
- Extension methods from Terraria classes
- A generic reflection wrapper
- Notification templates
- Generic Math
- Config Porting tools

# Changelog

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
- Added hover tooltit to InGameNotification
- Moved ReflectionHelper members to TypeExtensions
- Renamed method GetMember to GetPropertyFieldValue in TypeExtensions
- Removed methods GetMember, GetField and GetProperty from ReflectionHelper
- Replaced Joined\<TList, T> by JoinedLists\<T>
- Added class PortMember
- Added ItemSlots constants
- Added class ToFromStringConverter
- Improved Json Serilization
- Fixed a bug with MultiChoice tooltip
- Fixed Multichoice swap button labels
- Fixed bugs with Range and RangeSet

### v1.0.1
- Fixed mod not loading on the server
- Made MathX generic
- Added MathX.Snap

### v1.0
- Initial release on tML