#include <iostream>
#include <fstream>
#include <string>
#include <stdexcept>
#include <Windows.h>

void hide_byte_into_pixel(RGBQUAD& pixel, uint8_t hideByte) {
	pixel.rgbBlue &= (0xFC);
	pixel.rgbBlue |= (hideByte >> 6) & 0x3;
	pixel.rgbGreen &= (0xFC);
	pixel.rgbGreen |= (hideByte >> 4) & 0x3;
	pixel.rgbRed &= (0xFC);
	pixel.rgbRed |= (hideByte >> 2) & 0x3;
	pixel.rgbReserved &= (0xFC);
	pixel.rgbReserved |= (hideByte) & 0x3;
}

uint8_t get_hided_byte_from_pixel(RGBQUAD pixel) {
	//std::cout << (int)pixel.rgbBlue << '\t' 
	//	<< (int)pixel.rgbGreen << '\t' 
	//	<< (int)pixel.rgbRed << '\t' 
	//	<< (int)pixel.rgbReserved << '\t';

	uint8_t hided_byte = pixel.rgbBlue & 0x3;
	hided_byte <<= 2;
	hided_byte |= pixel.rgbGreen & 0x3;
	hided_byte <<= 2;
	hided_byte |= pixel.rgbRed & 0x3;
	hided_byte <<= 2;
	hided_byte |= pixel.rgbReserved & 0x3;

	//std::cout << (int)hided_byte << '\n';
	return hided_byte;
}

template <class T>
inline void read(std::istream& is, T& result) {
	is.read(reinterpret_cast<char*>(&result), sizeof(result));
}

template <class T>
inline void write(std::ostream& os, T& text) {
	os.write(reinterpret_cast<char*>(&text), sizeof(text));
}

const auto in_flags = std::ios_base::in | std::ios_base::binary;
const auto out_flags = std::ios_base::out | std::ios_base::binary;

void decode(const std::string& encoded_bmp, const std::string& hided_txt) {
	std::ifstream bmp(encoded_bmp, in_flags);
	std::ofstream txt(hided_txt, out_flags);
	if (!bmp || !txt)
		throw std::invalid_argument("Wrong file name(s)");

	bmp.seekg(sizeof(BITMAPFILEHEADER) + sizeof(BITMAPINFOHEADER));

	RGBQUAD pixel;
	while (true) {
		read(bmp, pixel);
		auto byte = get_hided_byte_from_pixel(pixel);
		if (byte == 0xFF)
			break;

		txt << byte;
	}
	txt.close();
	bmp.close();
}

void encode(const std::string& source_bmp, const std::string& txt, const std::string& result_bmp) {
	std::ifstream text(txt, in_flags);
	std::ifstream bmp(source_bmp, in_flags);
	std::ofstream new_bmp(result_bmp, out_flags);

	if (!bmp || !text || !new_bmp)
		throw std::invalid_argument("Wrong file name(s)");

	BITMAPFILEHEADER file_header;
	BITMAPINFOHEADER info_header;
	read(bmp, file_header);
	read(bmp, info_header);
	write(new_bmp, file_header);
	write(new_bmp, info_header);

	uint16_t modified_pixels = 0u;
	RGBQUAD pixel;
	while (true) {
		uint8_t txt_byte;
		read(text, txt_byte);
		if (text.eof())
			break;

		read(bmp, pixel);
		hide_byte_into_pixel(pixel, txt_byte);
		write(new_bmp, pixel);

		++modified_pixels;
	}

	read(bmp, pixel);
	hide_byte_into_pixel(pixel, 0xFF);
	write(new_bmp, pixel);
	++modified_pixels;

	for (size_t i = modified_pixels; i < info_header.biHeight * info_header.biWidth; ++i) {
		read(bmp, pixel);
		write(new_bmp, pixel);
	}

	text.close();
	bmp.close();
	new_bmp.close();
}

int main() {
	SetConsoleCP(1251);
	std::string q;
	while (true) {
		std::cout << "========\n'1' decode\n" <<
			"'2' encode\n" <<
			"else quit." << std::endl;
		std::cin >> q;

		if (q == "1") {//Decoding.
			std::string encoded_bmp, hided_txt;
			std::cout << "Enter:\n"
				<< "encoded bmp file: ";
			std::cin.get();
			std::getline(std::cin, encoded_bmp);
			std::cout << "output txt file: ";
			std::getline(std::cin, hided_txt);

			try {
				decode(encoded_bmp, hided_txt);
				std::cout << "decoding result -> " << hided_txt << std::endl;
			}
			catch (const std::exception& ex) {
				std::cout << ex.what() << std::endl;
			}

		}
		else if (q == "2") {//Encoding.
			std::string source_bmp, txt, new_bmp;
			std::cout << "Enter:\n"
				<< "source bmp file: ";
			std::cin.get();
			std::getline(std::cin, source_bmp);
			std::cout << "input txt file: ";
			std::getline(std::cin, txt);
			std::cout << "output bmp file: ";
			std::getline(std::cin, new_bmp);

			try {
				encode(source_bmp, txt, new_bmp);
				std::cout << "encoding result -> " << new_bmp << std::endl;
			}
			catch (const std::exception& ex) {
				std::cout << ex.what() << std::endl;
			}

		}
		else {
			break;
		}
	}

	return 0;
}