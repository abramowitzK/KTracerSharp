PHONY: run build clean

run : build
	@#Tried this, doesn't make any difference
	@#@mono --aot -O=all ./Graphics.exe
	@./Graphics.exe ${ARGS}
	@./ConvertToPng.sh
	@rm -f out.ppm
	@rm -f intensity.ppm
build :
	@mcs  -r:OpenTK.dll *.cs -out:Graphics.exe
clean : 
	@rm -f Graphics.exe
	@rm -f out.png
	@rm -f intensity.png
