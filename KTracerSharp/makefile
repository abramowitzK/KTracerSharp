PHONY: run build clean

run : build
	@#Tried this, doesn't make any difference
	@#@mono --aot -O=all ./Graphics.exe
	@./Graphics.exe ${ARGS}
	@./ConvertToPng.sh
	@rm out.ppm
build :
	@dmcs  -r:OpenTK.dll *.cs -out:Graphics.exe
clean : 
	@rm Graphics.exe
	@rm out.png
