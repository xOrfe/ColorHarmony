# Changelog

## 0.3.1

- New version is dope :c

### Migration notes

Custom HLSL include paths change from Editor/Shaders to Runtime/Shaders. Editor APIs use XO.ColorHarmony.Editor; runtime APIs remain XO.ColorHarmony. Existing Harmonica class-name settings continue to resolve.

Corrected alpha, brightness and circular interpolation may change legacy bakes. Review output before applying. Legacy source subassets stay in the original material file; label-based source lookup remains necessary until migrated.
