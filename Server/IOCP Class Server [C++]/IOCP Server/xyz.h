#pragma once
#ifndef __xyz_H__
#define __xyz_H__

#include <cmath>

class xyz {

public:

	union {
		float data[3];
		struct {
			float x;
			float y;
			float z;
		};
	};

	// Constructors

	// xyztors default to 0, 0, 0.
	xyz() {
		x = 0;
		y = 0;
		z = 0;
	}

	// Construct with values, 3D
	xyz(float ax, float ay, float az) {
		x = ax;
		y = ay;
		z = az;
	}

	// Construct with values, 2D
	xyz(float ax, float ay) {
		x = ax;
		y = ay;
		z = 0;
	}

	// Copy constructor
	xyz(const xyz& o) {
		x = o.x;
		y = o.y;
		z = o.z;
	}

	// Addition

	xyz operator+(const xyz& o) {
		return xyz(x + o.x, y + o.y, z + o.z);
	}

	xyz& operator+=(const xyz& o) {
		x += o.x;
		y += o.y;
		z += o.z;
		return *this;
	}

	// Subtraction

	xyz operator-() {
		return xyz(-x, -y, -z);
	}

	xyz operator-(const xyz o) {
		return xyz(x - o.x, y - o.y, z - o.z);
	}

	xyz& operator-=(const xyz o) {
		x -= o.x;
		y -= o.y;
		z -= o.z;
		return *this;
	}

	// Multiplication by scalars

	xyz operator*(const float s) {
		return xyz(x * s, y * s, z * s);
	}

	xyz& operator*=(const float s) {
		x *= s;
		y *= s;
		z *= s;
		return *this;
	}

	// Division by scalars

	xyz operator/(const float s) {
		return xyz(x / s, y / s, z / s);
	}

	xyz& operator/=(const float s) {
		x /= s;
		y /= s;
		z /= s;
		return *this;
	}

	// Dot product

	float operator*(const xyz o) {
		return (x * o.x) + (y * o.y) + (z * o.z);
	}

	// An in-place dot product does not exist because
	// the result is not a xyztor.

	// Cross product

	xyz operator^(const xyz o) {
		float nx = y * o.z - o.y * z;
		float ny = z * o.x - o.z * x;
		float nz = x * o.y - o.x * y;
		return xyz(nx, ny, nz);
	}

	xyz& operator^=(const xyz o) {
		float nx = y * o.z - o.y * z;
		float ny = z * o.x - o.z * x;
		float nz = x * o.y - o.x * y;
		x = nx;
		y = ny;
		z = nz;
		return *this;
	}

	// Other functions

	// Length of xyztor
	float magnitude() {
		return sqrt(magnitude_sqr());
	}

	// Length of xyztor squared
	float magnitude_sqr() {
		return (x * x) + (y * y) + (z * z);
	}

	// Returns a normalised copy of the xyztor
	// Will break if it's length is 0
	xyz normalised() {
		return xyz(*this) / magnitude();
	}

	// Modified the xyztor so it becomes normalised
	xyz& normalise() {
		(*this) /= magnitude();
		return *this;
	}

};
#endif