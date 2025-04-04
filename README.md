### No-Code Mod Configs
Mods can now add basic configs without having to make a code mod, with the ability to use any/most mod APIs.

__Creating a Config__
- Create the file `MOD_FOLDER/ReMIX/Config/config.yaml` in your mod.
- Add a setting.
```
settings:
    - id: EnableBgme            # ID you'll use to get this setting.
      type: toggle              # Types: number, text, toggle
      default: true             # Default value for setting.
      name: Enable BGME         # (Optional) Setting name to show in config.
      category: Music           # (Optional) Setting category.
      description: Description. # (Optional) Setting description.
```
- Add an action.
```
actions:
    - using: BGME.Framework.Interfaces.IBgmeApi # The full name of the API to use.
      if: EnableBgme  # (Optional) Setting that enable/disables this action.
      run: AddPath    # API function to run.
      with:           # The arguments to run it with.
        - '{ModDir}/new-bgme/script.pme'
```

- __Remember to add a Mod Dependency on any mod whose APIs you use!__ 
- Done 🎉! A mod config should now be available, though you might have to refresh the mod list or restart Reloaded.

__Constants and Placeholders__

You can insert constants and settings into any part of an action, besides the `if` condition. The syntax for it is `{VariableName}`.  If you do, make sure to wrap the entire text with `''`.

```
constants:
    BGME_API: BGME.Framework.Interfaces.IBgmeApi
    RYO_API: Ryo.Interfaces.IRyoApi
    
settings:
    - id: EnableBgme
      category: Music
      name: Enable BGME
      description: A description of this setting.
      type: toggle
      default: true
      
    - id: NumberSetting
      category: Category 1
      type: number
      default: 123

actions:
    - using: '{BGME_API}'
      if: EnableBgme
      run: AddPath
      with:
        - '{ModDir}/new-bgme/{NumberSetting}.pme'
        
    - using: '{RYO_API}'
      if: EnableBgme
      run: AddAudioPath
      with:
        - '{ModDir}/new-ryo/'
        - null
```
