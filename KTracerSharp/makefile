PHONY: run build clean

run : build
	@./Graphics.exe ${ARGS}
build :
	@dmcs -lib: -r:OpenTK.dll Program.cs -out:Graphics.exe
clean : 
	@rm Graphics.exe
