# Animated Cybergrind Textures

### Allows you to use animated custom textures in Cybergrind mode

## Basics

### Important note

Please check out the information on [supported texture file formats](#supported-formats) first.

### Usage
After installing the mod, place your animated textures into the Cybergrind custom textures folder or any of its subfolders (`ULTRAKILL\Cybergrind\Textures`) as you would normally do.

In game, get to the blue Cybergrind terminal and open custom textures section.
Animated textures should now be visible in the Terminal browser and have a little "📹" icon distinguishing them from the regular static textures. Thumbnails aren't animated in file browser, but don't worry, they will be on a little preview in the left corner of the screen.

The same also goes for the skybox textures. _Animated glow textures, however, are not supported._

![Texture preview](https://github.com/user-attachments/assets/ac3b4c29-7d6b-4b84-b499-998a907ce827)

_Example of a working animated texture in a preview section_

### Preconditions
In order for your textures to work properly, please note the following:

- Texture format must meet the [requirements](#supported-formats)
- Grid texture must have a 1:1 aspect ratio. 100x100 is the most optimal resolution since it's used by the renderer
- Skybox texture must have a 2:1 aspect ratio (1920x960 resolution is used by renderer and thus is preferable)
- For skybox textures, an [equirectangular projection](https://en.wikipedia.org/wiki/Equirectangular_projection) is used, just like for any other skybox texture in ULTRAKILL

![Skybox example](https://github.com/user-attachments/assets/409533d1-9633-40f9-89b4-2d7e17f0db2f)

_Example of a custom animated skybox texture_

### Skybox rotation toggle

The mod also introduces an option to disable a skybox spin (enabled by default)

![Skybox rotation](https://github.com/user-attachments/assets/02079847-23e9-4846-aa87-c2496566b1f4)

_I find spinning skyboxes extremely annoying ngl_

## Supported formats

It's worth mentioning right away that this mod only operates with video files, which means _**GIFs are not supported**_ and you're gonna have to have your textures converted into video files first (more on available options [below](#acquiring-the-textures))

On a Windows platform, the following formats are supported:

`
.asf, .avi, .dv, .m4v, .mov, .mp4, .mpg, .mpeg, .ogv, .vp8, .webm, .wmv
`

As for the codec, `H.264` would be the codec of choice but the video player support is not limited to it. See [the official Unity docs](https://docs.unity3d.com/Manual/VideoSources-FileCompatibility.html) for more information on supported codecs and formats.

## Acquiring the textures

Okay, supporting textures animation is one thing, but how do I get these, I hear you ask? And what do I do with my GIFs?

I'm aware that GIF is by far the most requested format, so here's two options you can choose from:

1. Use `GIF -> MP4` online converters. There's a wide variety of these, most of them allow you to choose between different codecs, resolutions etc (just a reminder that `H.264` is preferable). For instance, you can use [this one](https://cloudconvert.com/gif-to-mp4)
2. Using a standalone software. It could be a video editor of your choice or some specialized media framework. Personally, I prefer `ffmpeg` over other alternatives. It may be confusing to use, but it is extremely agile. I'll provide some examples that could potentially help you down below

### Skyboxes

Any equirectangular panorama video can be used as skybox as long as its resolution is 2:1. I strongly advise to keep it `1920x960` because that's the resolution of a skybox texture and any other resolution can lead to performance issues.

**Important!** If you've downloaded a 360 video from YouTube, you're gonna have to convert it from EAC to equirectangular in order for it to work properly (see [ffmpeg section](#ffmpeg-is-your-best-friend))

### ffmpeg is your best friend

Like I mentioned before, `ffmpeg` spares you lots of troubles working with the media files. In order to install it:

1. Download the latest `ffmpeg` build [from here](https://github.com/BtbN/FFmpeg-Builds/releases/download/latest/ffmpeg-master-latest-win64-gpl.zip) or [here](https://www.gyan.dev/ffmpeg/builds/ffmpeg-git-full.7z)
2. Find `ffmpeg.exe` executable in the downloaded archive in the `bin` folder and extract it somewhere on your PC (I'd recommend to use an empty folder for that in order to easily keep track of your files)
3. Open a command line in this folder

Now here's a couple examples of using it:

---
_Convert a GIF animation into a 100x100 .mp4 (it may not necessarily be a gif file, this also works with most of the video formats):_

`ffmpeg -i "INPUT_FILE_NAME.gif" -vf "scale=100:100" -movflags faststart -pix_fmt yuv420p -c:v libx264 -crf 25 "OUTPUT_FILE_NAME.mp4"`

---

_Change resolution of a video to 1920x960_:

`ffmpeg -i "INPUT_FILE_NAME.mp4" -vf "scale=1920:960" -c:v libx264 -crf 25 -preset fast -c:a copy "OUTPUT_FILE_NAME.mp4"`

---

_Convert downloaded 360 YouTube video from EAC to EQU (for skyboxes)_

`ffmpeg -i "INPUT_FILE_NAME.mp4" -vf "v360=eac:equirect,scale=1920:960" "OUTPUT_FILE_NAME.mp4"`

## Configuration

I wouldn't generally recommend doing this unless you're certain of your actions. You can modify a target resolution of grid/skybox textures which will be used by the renderer (e.g. you want to use 4K video as a skybox and you want it to look sharp), but bare in mind it can severely impact the performance.

You can find a corresponding config at `ULTRAKILL\BepInEx\config\AnimatedCybergrindTextures\config.cfg` (created after the first usage of the mod).