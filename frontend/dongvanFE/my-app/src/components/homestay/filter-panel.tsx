"use client"

import { Card, CardContent, CardHeader, CardTitle } from "../ui/card"
import { Slider } from "../ui/slider"
import { Checkbox } from "../ui/checkbox"
import { Label } from "../ui/label"
import { Star } from "../../lib/icons"

interface FilterPanelProps {
  filters: {
    priceRange: [number, number]
    rating: number
    amenities: string[]
  }
  onFilterChange: (filters: {
    priceRange: [number, number]
    rating: number
    amenities: string[]
  }) => void
}

const amenitiesList = [
  "WiFi",
  "Pool",
  "Gym",
  "Restaurant",
  "Spa",
  "Beach Access",
  "Bar",
  "Parking",
  "Room Service",
  "Business Center",
]

export default function FilterPanel({ filters, onFilterChange }: FilterPanelProps) {
  const handlePriceChange = (value: number[]) => {
    onFilterChange({
      ...filters,
      priceRange: [value[0], value[1]],
    })
  }

  const handleRatingChange = (ratingValue: number, checked?: boolean) => {
    const newRating = typeof checked === "boolean" ? (checked ? ratingValue : 0) : filters.rating === ratingValue ? 0 : ratingValue
    onFilterChange({
      ...filters,
      rating: newRating,
    })
  }

  const handleAmenityChange = (amenity: string, checked: boolean) => {
    const newAmenities = checked ? [...filters.amenities, amenity] : filters.amenities.filter((a) => a !== amenity)

    onFilterChange({
      ...filters,
      amenities: newAmenities,
    })
  }

  const formatPrice = (price: number) => {
    return new Intl.NumberFormat("vi-VN", {
      style: "currency",
      currency: "VND",
    }).format(price)
  }

  return (
    <Card className="sticky top-24">
      <CardHeader>
        <CardTitle className="text-lg font-semibold">Bộ lọc tìm kiếm</CardTitle>
      </CardHeader>
      <CardContent className="space-y-6">
        {/* Price Range Filter */}
        <div>
          <Label className="text-sm font-medium mb-3 block">
            Khoảng giá ({formatPrice(filters.priceRange[0])} - {formatPrice(filters.priceRange[1])})
          </Label>
          <Slider
            value={filters.priceRange}
            onValueChange={handlePriceChange}
            max={5000000}
            min={0}
            step={100000}
            className="w-full"
          />
          <div className="flex justify-between text-xs text-muted-foreground mt-2">
            <span>0đ</span>
            <span>5,000,000đ</span>
          </div>
        </div>

        {/* Rating Filter */}
        <div>
          <Label className="text-sm font-medium mb-3 block">Đánh giá tối thiểu</Label>
          <div className="space-y-2">
            {[5, 4, 3, 2, 1].map((rating) => (
              <div key={rating} className="flex items-center space-x-2">
                <Checkbox
                  id={`rating-${rating}`}
                  checked={filters.rating === rating}
                  onCheckedChange={(checked: boolean) => handleRatingChange(rating, checked)}
                />
                <Label htmlFor={`rating-${rating}`} className="flex items-center cursor-pointer">
                  <div className="flex items-center">
                    {Array.from({ length: rating }).map((_, i) => (
                      <Star key={i} className="h-4 w-4 fill-yellow-400 text-yellow-400" />
                    ))}
                    {Array.from({ length: 5 - rating }).map((_, i) => (
                      <Star key={i} className="h-4 w-4 text-gray-300" />
                    ))}
                  </div>
                  <span className="ml-2 text-sm">từ {rating} sao</span>
                </Label>
              </div>
            ))}
          </div>
        </div>

        {/* Amenities Filter */}
        <div>
          <Label className="text-sm font-medium mb-3 block">Tiện nghi</Label>
          <div className="space-y-2 max-h-48 overflow-y-auto">
            {amenitiesList.map((amenity) => (
              <div key={amenity} className="flex items-center space-x-2">
                <Checkbox
                  id={`amenity-${amenity}`}
                  checked={filters.amenities.includes(amenity)}
                  onCheckedChange={(checked: boolean) => handleAmenityChange(amenity, checked)}
                />
                <Label htmlFor={`amenity-${amenity}`} className="text-sm cursor-pointer">
                  {amenity}
                </Label>
              </div>
            ))}
          </div>
        </div>
      </CardContent>
    </Card>
  )
}
