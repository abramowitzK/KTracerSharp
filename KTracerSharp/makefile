PHONY: run build clean

run : build
	@./Graphics.exe ${ARGS}
	@./ConvertToPng.sh
	@rm out.ppm
build :
	@dmcs  -r:OpenTK.dll *.cs -out:Graphics.exe
clean : 
	@rm Graphics.exe
