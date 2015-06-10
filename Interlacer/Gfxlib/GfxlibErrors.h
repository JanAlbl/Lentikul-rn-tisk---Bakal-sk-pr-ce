#define _GFXLIB_ERRORS_H

//vyctovy typ pro ruzne druhy chyb
enum GfxlibErrors
{
	PictureLoadFailure,  //chyba nacteni obrazku
	PictureSaveFailure,  //chyba ulozeni obrazku
	PictureCreationFailure,  //chyba vytvoreni obrazku
	PictureResizeFilure,  //chyba zmeny velikosti obrazku
	PictureClipFailure,  //chyba oriznuti obrazku
	PictureWrongFormat,  //chybny format obrazku
	OutOfMemory  //nedostatek pameti (prekroceni limitu magicku)
};