/*
Licensed under the ImageMagick License (the "License"); you may not use
this file except in compliance with the License.  You may obtain a copy
of the License at

http://www.imagemagick.org/script/license.php

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the
License for the specific language governing permissions and limitations
under the License.
*/

#include "stdafx.h"

#include "Gfxlib.h"
#include "GfxlibErrors.h"
#include <fstream>
#include <Magick++.h>

//funkce pro zapsani chyby do souboru, nezapisuje v rezimu append,
//takze je dobre vzdy zkontrolovat datum vytvoreni souboru a presvedcit se, ze to neni nejaka stara chyba
static void writeError(std::string str)
{
	std::ofstream file("Error.txt");
	file << str;
	file.close();
}

//funkce pro zapsani varovani do souboru, nezapisuje v rezimu append,
//takze je dobre vzdy zkontrolovat datum vytvoreni souboru a presvedcit se, ze to neni nejake stare varovani
static void writeWarning(std::string str)
{
	std::ofstream file("Warning.txt");
	file << str;
	file.close();
}

//funkce pro test spojeni, slouzi pouze pro vyzkouseni, jestli je knihovna spravnce nactena v klientske aplikace
void test() {}

//nastaveni pametovych limitu knihovny
void setResourceLimits()
{
#if defined _WIN64
	//nastaveni pametovych limitu pro 64-bitovy release
	Magick::ResourceLimits::memory(68719476736);  //nastaveni limitu pro celou pamet na 64GB
	Magick::ResourceLimits::area(68719476736);  //nastaveni limitu pro samostatnou alokaci na 64GB
#else
	//nastaveni pametovych limitu pro 32-bitovy release
	Magick::ResourceLimits::memory(4294967296);  //nastaveni limitu pro celou pamet na 4GB
	Magick::ResourceLimits::area(4294967296);  //nastaveni limitu pro samostatnou alokaci na 4GB
#endif
	Magick::ResourceLimits::disk(0);  //vypnuti cachovani na disk, v pripade nedostatku pameti data na disk odswapuje OS,
									  //ktery to dela mnohem rychleji a spolehliveji
}

//vytvori instanci tridy Magick::Image o dane sirce a vysce a vrati ji jako void* pointer
void* createImage(int w, int h)
{
	std::string wStr = std::to_string(w);
	std::string hStr = std::to_string(h);
	std::string sizeStr = wStr + std::string("x") + hStr;  //nova velikost se predava jako string, napr "600x400"
	Magick::Image* img = nullptr;
	try
	{
		img = new Magick::Image;
		img->size(sizeStr);  //nastaveni velikosti
	}
	catch (Magick::ErrorCache & er)  //prekroceni pametovych limitu
	{
		writeError(er.what());
		delete img;
		RaiseException(GfxlibErrors::OutOfMemory, 0, 0, 0);
	}
	catch (Magick::Error & er)  //ostatni chyby magicku
	{
		writeError(er.what());
		delete img;
		RaiseException(GfxlibErrors::PictureCreationFailure, 0, 0, 0);
	}
	catch (...)  //ostatni chyby
	{
		delete img;
		RaiseException(GfxlibErrors::PictureCreationFailure, 0, 0, 0);
	}
	return (void*)img;
}

//smaze danou instnanci tridy Magick::Image
void deleteImage(void* ptr)
{
	Magick::Image* imagePtr = (Magick::Image*)ptr;
	delete imagePtr;
}

//vrati pole pixelu dane instance tridy Magick::Image, ktera je predana parametrem
INTPX* getPixelDataPtr(void* ptr)
{
	Magick::Image* imagePtr = (Magick::Image*)ptr;
	INTPX* pxPtr = nullptr;
	try
	{
		pxPtr = (INTPX*)imagePtr->getPixels(0, 0, imagePtr->columns(), imagePtr->rows());  //ziskani ukazatele na pocatek pole pixelu
	}
	catch (Magick::ErrorCache & er)  //prekroceni pametovych limitu
	{
		writeError(er.what());
		RaiseException(GfxlibErrors::OutOfMemory, 0, 0, 0);
	}
	catch (Magick::Error & er)  //ostatni chyby magicku
	{
		writeError(er.what());
		RaiseException(GfxlibErrors::PictureCreationFailure, 0, 0, 0);
	}
	catch (...)  //ostatni chyby
	{
		RaiseException(GfxlibErrors::PictureCreationFailure, 0, 0, 0);
	}
	return pxPtr;
}

//nacte obrazek ze souboru, jehoz nazev je predan parametrem a vrati ho v podobe instance tridy Magick::Image
void* loadImage(char* filename)
{
	Magick::Image* img = nullptr;
	char* data = nullptr;  
	std::ifstream file;
	try
	{
		file.open(filename, std::ios::binary);
		file.seekg(0, std::ios::end);
		long length = (long)file.tellg();
		file.seekg(0, std::ios::beg);
		data = new char[length];  //buffer pro nactena binarni data ze souboru
		file.read(data, length);  //nacteni souboru binarne
		file.close();
		img = new Magick::Image;
		img->read(Magick::Blob(data, length));  //nacteni magickem z pameti, nelze nacist primo ze souboru
												//nastal by problem, pokud je v nazvu diakritika, protoze magick pouziva UTF-8 kodovani
		delete[] data;
	}
	catch (Magick::Error & er)  //chyba pri nacitani obrazku magickem z pameti
	{
		writeError(er.what());
		if (file.is_open())
			file.close();
		delete[] data;
		delete img;
		RaiseException(GfxlibErrors::PictureWrongFormat, 0, 0, 0);
	}
	catch (Magick::Warning & w)  //odchyceni varovani magicku, v tomto pripade se pouze zapise info do souboru a pokracuje se dal
	{
		writeWarning(w.what());
		if (file.is_open())
			file.close();
		delete[] data;
	}
	catch (std::exception & ex)  //chyba pri binarnim nacitani tridou istream
	{
		if (file.is_open())
			file.close();
		writeError(ex.what());
		delete[] data;
		delete img;
		RaiseException(GfxlibErrors::PictureLoadFailure, 0, 0, 0);
	}
	catch (...)  //ostatni chyby
	{
		delete img;
		delete[] data;
		if (file.is_open())
			file.close();
		RaiseException(GfxlibErrors::PictureLoadFailure, 0, 0, 0);
	}
	return (void*)img;
}

//nacte obrazek bez obrazovych dat (pouze info) ze souboru, jehoz nazev je predan parametrem a vrati ho v podobe instance tridy Magick::Image
void* pingImage(char* filename)
{
	Magick::Image* img = nullptr;
	char* data = nullptr;
	std::ifstream file;
	try
	{
		file.open(filename, std::ios::binary);
		file.seekg(0, std::ios::end);
		long length = (long)file.tellg();
		file.seekg(0, std::ios::beg);
		data = new char[length];  //buffer pro nactena binarni data ze souboru
		file.read(data, length);  //nacteni souboru binarne
		file.close();
		img = new Magick::Image;
		img->ping(Magick::Blob(data, length));  //nacteni magickem z pameti, nelze nacist primo ze souboru
												//nastal by problem, pokud je v nazvu diakritika, protoze magick pouziva UTF-8 kodovani
		delete[] data;
	}
	catch (Magick::Error & er)  //chyba pri nacitani obrazku magickem z pameti
	{
		writeError(er.what());
		if (file.is_open())
			file.close();
		delete[] data;
		delete img;
		RaiseException(GfxlibErrors::PictureWrongFormat, 0, 0, 0);
	}
	catch (Magick::Warning & w)  //odchyceni varovani magicku, v tomto pripade se pouze zapise info do souboru a pokracuje se dal
	{
		writeWarning(w.what());
		if (file.is_open())
			file.close();
		delete[] data;
	}
	catch (std::exception & ex)  //chyba pri binarnim nacitani tridou istream
	{
		if (file.is_open())
			file.close();
		writeError(ex.what());
		delete[] data;
		delete img;
		RaiseException(GfxlibErrors::PictureLoadFailure, 0, 0, 0);
	}
	catch (...)  //ostatni chyby
	{
		delete img;
		delete[] data;
		if (file.is_open())
			file.close();
		RaiseException(GfxlibErrors::PictureLoadFailure, 0, 0, 0);
	}
	return (void*)img;
}

//ulozi obrazek do souboru, jehoz nazev je predan parametrem
void saveImage(void* ptr, char* filename)
{
	std::ofstream file;
	try
	{
		Magick::Image* imagePtr = (Magick::Image*)ptr;
		imagePtr->syncPixels();  //synchronizace pixelu z cache (nejsem si jisty, jestli je to nutne)
		Magick::Blob blob;  //vytvoreni pametoveho blobu pro zapsani obrazku magickem
		imagePtr->compressType(MagickCore::CompressionType::LZWCompression);  //nastaveni komrese pro tif na LZW
		std::string filenameStr = std::string(filename);
		std::string ext = filenameStr.substr(filenameStr.find_last_of(".") + 1);  //ziskani koncovky, podle ktere magick pozna, do jakeho formatu ma obrazek ulozit
		imagePtr->write(&blob, ext);  //vytvoreni vystupniho obrazku, jen ne do souboru, ale do pametoveho blobu, nelze zapsat primo do souboru
									  //nastal by problem, pokud je v nazvu diakritika, protoze magick pouziva UTF-8 kodovani
		file.open(filename, std::ios::binary);
		file.write((const char*)blob.data(), blob.length());  //zapsani vystupniho obrazku binarne do pamezi z blobu
		file.close();
	}
	catch (Magick::Error & er)  //chyba magicku
	{
		writeError(er.what());
		if (file.is_open())
			file.close();
		RaiseException(GfxlibErrors::PictureSaveFailure, 0, 0, 0);
	}
	catch (std::exception & ex)  //ostatni chyby, jejichz vyjimky lze pretypovat ne exception
	{
		writeError(ex.what());
		if (file.is_open())
			file.close();
		RaiseException(GfxlibErrors::PictureSaveFailure, 0, 0, 0);
	}
	catch (...)  //ostatni chyby
	{
		if (file.is_open())
			file.close();
		RaiseException(GfxlibErrors::PictureSaveFailure, 0, 0, 0);
	}
}

//vrati sirku v pixelech instance tridy Magick::Image, ktera je predana parametrem
int getImageWidth(void* ptr)
{
	Magick::Image* imagePtr = (Magick::Image*)ptr;
	return (int)imagePtr->columns();
}

//vrati vysku v pixelech instance tridy Magick::Image, ktera je predana parametrem
int getImageHeight(void* ptr)
{
	Magick::Image* imagePtr = (Magick::Image*)ptr;
	return (int)imagePtr->rows();
}

//vrati rozliseni pro osu X instance tridy Magick::Image, ktera je preadana parametrem,
//druhy parametr je vystupni a je do nej ulozena informace o jednotkach
double getImageXResolution(void* ptr, int* unitType)
{
	Magick::Image* imagePtr = (Magick::Image*)ptr;
	double resolution = imagePtr->xResolution();  //ziskani rozliseni na ose X v danych jednotkach
	MagickCore::ResolutionType resType = imagePtr->resolutionUnits();  //zjisteni jednotek
	switch (resType)  //nastaveni vystupniho parametru unitType na hodnotu podle jednotek
	{
		case MagickCore::ResolutionType::PixelsPerInchResolution:  //1 pro DPI
			*unitType = 1;
			return resolution;
		case MagickCore::ResolutionType::PixelsPerCentimeterResolution:  //2 pro DPCM
			*unitType = 2;
			return resolution;
		default:  //pro nezname jednotky taktez 1, tudiz hodnota je brana jako DPI
			*unitType = 1;
			return resolution;
	}
}

//vrati rozliseni pro osu Y instance tridy Magick::Image, ktera je preadana parametrem,
//druhy parametr je vystupni a je do nej ulozena informace o jednotkach
double getImageYResolution(void* ptr, int* unitType)
{
	Magick::Image* imagePtr = (Magick::Image*)ptr;
	double resolution = imagePtr->yResolution();  //ziskani rozliseni na ose Y v danych jednotkach
	MagickCore::ResolutionType resType = imagePtr->resolutionUnits();  //zjisteni jednotek
	switch (resType)  //nastaveni vystupniho parametru unitType na hodnotu podle jednotek
	{
	case MagickCore::ResolutionType::PixelsPerInchResolution:  //1 pro DPI
		*unitType = 1;
		return resolution;
	case MagickCore::ResolutionType::PixelsPerCentimeterResolution:  //2 pro DPCM
		*unitType = 2;
		return resolution;
	default:  //pro nezname jednotky taktez 1, tudiz hodnota je brana jako DPI
		*unitType = 1;
		return resolution;
	}
}

//nastavi rozliseni pro instanci tridy Magick::Image, ktera je predana parametrem, posledni parametr je informace o jednotkach
void setImageResolution(void* ptr, double xRes, double yRes, int unitType)
{
	Magick::Image* imagePtr = (Magick::Image*)ptr;
	imagePtr->resolutionUnits(unitType == 1 ?  //nastaveni jednotek podle parametru unitType (1 - DPI, 2 - DPCM)
		MagickCore::ResolutionType::PixelsPerInchResolution : MagickCore::ResolutionType::PixelsPerCentimeterResolution);
	imagePtr->image()->x_resolution = xRes;
	imagePtr->image()->y_resolution = yRes;
}

//zmeni velikost obrazku v instanci tridy Magick::Image predane parametrem, posledni parametr je cislo pouziteho filtru
void resizeImage(void* ptr, int w, int h, int filterType)
{
	try
	{
		std::string wStr = std::to_string(w);
		std::string hStr = std::to_string(h);
		std::string sizeStr = wStr + std::string("x") + hStr + std::string("!");  //nova velikost se predava jako string, napr "600x400!", bez vykricniku by byl zachovan pomer stran
		Magick::Image* imagePtr = (Magick::Image*)ptr;
		imagePtr->syncPixels();  //synchronizace pixelu z cache (nejsem si jisty, jestli je to nutne)
		imagePtr->filterType((MagickCore::FilterTypes)filterType);  //nastaveni filtru
		imagePtr->resize(sizeStr);
	}
	catch (Magick::ErrorCache & er)  //prekroceni pametovych limitu
	{
		writeError(er.what());
		RaiseException(GfxlibErrors::OutOfMemory, 0, 0, 0);
	}
	catch (Magick::Error & er)  //ostatni chyby magicku
	{
		writeError(er.what());
		RaiseException(GfxlibErrors::PictureResizeFilure, 0, 0, 0);
	}
	catch (...)  //ostatni chyby
	{
		RaiseException(GfxlibErrors::PictureResizeFilure, 0, 0, 0);
	}
}

//orizne obrazek v instanci tridy Magick::Image predane parametrem na dany obdelnik
void clipImage(void* ptr, int x, int y, int w, int h)
{
	try
	{
		Magick::Image* imagePtr = (Magick::Image*)ptr;
		imagePtr->syncPixels();  //synchronizace pixelu z cache (nejsem si jisty, jestli je to nutne)
		imagePtr->crop(Magick::Geometry(w, h, x, y));
	}
	catch (Magick::Error & er)  //chyby magicku
	{
		writeError(er.what());
		RaiseException(GfxlibErrors::PictureClipFailure, 0, 0, 0);
	}
	catch (...)  //ostatni chyby
	{
		RaiseException(GfxlibErrors::PictureClipFailure, 0, 0, 0);
	}
}