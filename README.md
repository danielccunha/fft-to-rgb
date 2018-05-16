# Using FFT from audio to animate an analog RGB LED strip

In this project, we created a program that captures the audio coming from the sound card of a computer("Speakers" in the Windows Playback Devices tab), analyzes it using the [Fast Fourier Transform(FFT)](https://en.wikipedia.org/wiki/Fast_Fourier_transform) algorithm, converts data from specific frequencies previously configured by us and the amplitude of the sound in color patterns([RGB standard](https://en.wikipedia.org/wiki/RGB_color_model)) and luminous intensity, sends the converted data via USB/Wi-Fi to an [Arduino UNO](https://store.arduino.cc/usa/arduino-uno-rev3) connected in a circuit with [MOSFETs](https://en.wikipedia.org/wiki/MOSFET), which activates the LED strip, according to the sound.


###### This project is at the beginning of its development. The code and instructions for use will be made available as soon as possible.


## Credits
Initial program based on codes developed by [Scott W Harden](https://github.com/swharden).

## License
None (Yet).
