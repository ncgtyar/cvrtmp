# CVRTMP
## Realtime object detection from RTMP stream
*See:*
- https://github.com/shimat/opencvsharp
- https://github.com/ncgtyar/localrtmp
- https://github.com/nginx/nginx

### CVRTMP allows you to perform realtime object detection on the screen using a local RTMP stream with the OpenCV wrapper [OpenCVSharp](https://github.com/shimat/opencvsharp)

## Usage
```
using CVRTMP;

(...)

string onnxModelPath = "";
string[] classNames = { };
string rtmpUrl = "rtmp://localhost/live/cvrtmp"

cvrtmp.init(onnxModelPath, classNames, rtmpUrl);
cvrtmp.start();

while (true)
{
    foreach (Object obj in cvrtmp.objects)
    {
        Console.WriteLine($"[{obj.label}] x:{obj.x}, y:{obj.y}, w:{obj.w}, h:{obj.h}");
    }
}
```

- `Object` is a class containing the `class label`, `x`, `y`, `w` and `h` fields.
- `cvrtmp.init();` calls `RTMP.open()`, which opens the RTMP stream using **OpenCVSharp**'s `VideoCapture`.
- `cvrtmp.start();` starts the loop that continuously updates the list of detected objects - `objects` field of `cvrtmp`. You don't need to do anything to update it, it's all automatically handled.

## Notes
- Your model must be in **ONNX** format.
- If you plan to use streaming apps such as [OBS Studio](https://github.com/obsproject/obs-studio), you must have a local RTMP server running. I recommend [Nginx](https://github.com/nginx/nginx). *You can ignore this if your program already handles the server itself.*
- *OpenCV installation on the machine is **not** required. It's included in [OpenCVSharp](https://github.com/shimat/opencvsharp).*
- *If you're encountering any issues with the stream, make sure it actually works using apps like [VLC](https://github.com/videolan/vlc)*.
