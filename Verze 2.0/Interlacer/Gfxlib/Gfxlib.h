#ifndef _GFXLIB_H
#define _GFXLIB_H
#include <iostream>

extern "C"
{
	/*definuje typ podle velikosti barevneho kanalu (INT32 - 8-bitovy kanal / INT64 - 16-bitovy kanal)
	pri zmene je dobre prepsat hodnotu makra MAGICKCORE_QUANTUM_DEPTH v souboru magick-baseconfig.h*/
	typedef INT32 INTPX;

	//funkce pro test spojeni, slouzi pouze pro vyzkouseni, jestli je knihovna spravnce nactena v klientske aplikace
	__declspec(dllexport) void test();

	//nastaveni pametovych limitu knihovny
	__declspec(dllexport) void setResourceLimits();

	//vytvori instanci tridy Magick::Image o dane sirce a vysce a vrati ji jako void* pointer
	__declspec(dllexport) void* createImage(int w, int h);

	//smaze danou instnanci tridy Magick::Image
	__declspec(dllexport) void deleteImage(void*);

	//vrati pole pixelu dane instance tridy Magick::Image, ktera je predana parametrem
	__declspec(dllexport) INTPX* getPixelDataPtr(void*);

	//nacte obrazek ze souboru, jehoz nazev je predan parametrem a vrati ho v podobe instance tridy Magick::Image
	__declspec(dllexport) void* loadImage(char*);

	//nacte obrazek bez obrazovych dat (pouze info) ze souboru, jehoz nazev je predan parametrem a vrati ho v podobe instance tridy Magick::Image
	__declspec(dllexport) void* pingImage(char*);

	//ulozi obrazek do souboru, jehoz nazev je predan parametrem
	__declspec(dllexport) void saveImage(void*, char*);

	//vrati sirku v pixelech instance tridy Magick::Image, ktera je predana parametrem
	__declspec(dllexport) int getImageWidth(void*);

	//vrati vysku v pixelech instance tridy Magick::Image, ktera je predana parametrem
	__declspec(dllexport) int getImageHeight(void*);

	//vrati rozliseni pro osu X instance tridy Magick::Image, ktera je preadana parametrem,
	//druhy parametr je vystupni a je do nej ulozena informace o jednotkach
	__declspec(dllexport) double getImageXResolution(void*, int*);

	//vrati rozliseni pro osu Y instance tridy Magick::Image, ktera je preadana parametrem,
	//druhy parametr je vystupni a je do nej ulozena informace o jednotkach
	__declspec(dllexport) double getImageYResolution(void*, int*);

	//nastavi rozliseni pro instanci tridy Magick::Image, ktera je predana parametrem, posledni parametr je informace o jednotkach
	__declspec(dllexport) void setImageResolution(void*, double, double, int);

	//zmeni velikost obrazku v instanci tridy Magick::Image predane parametrem, posledni parametr je cislo pouziteho filtru
	__declspec(dllexport) void resizeImage(void*, int, int, int);

	//orizne obrazek v instanci tridy Magick::Image predane parametrem na dany obdelnik
	__declspec(dllexport) void clipImage(void*, int, int, int, int);
}

#endif