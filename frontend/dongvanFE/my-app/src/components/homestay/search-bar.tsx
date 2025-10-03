"use client"

import type React from "react"
import { useState } from "react"
import { Button } from "../ui/button"
import { Input } from "../ui/input"
import { Card } from "../ui/card"
import { Calendar } from "../ui/calendar"
import { Popover, PopoverContent, PopoverTrigger } from "../ui/popover"
// ...existing code...
import { Search, CalendarIcon, Users } from "../../lib/icons"
import { formatDate as format } from "../../lib/date"

interface SearchBarProps {
  onSearch: (query: string, checkIn: Date | null, checkOut: Date | null, guests: number) => void
}

export default function SearchBar({ onSearch }: SearchBarProps) {
  const [rooms, setRooms] = useState<number>(1)
  const [checkIn, setCheckIn] = useState<Date | null>(null)
  const [checkOut, setCheckOut] = useState<Date | null>(null)
  const [guests, setGuests] = useState<number>(2)

  const handleSearch = () => {
    // pass rooms as the first query parameter as a string for compatibility with existing handlers
    onSearch(rooms.toString(), checkIn, checkOut, guests)
  }

  return (
    <Card className="p-6 shadow-lg">
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-5 gap-4">
        {/* Check-in Date */}
        <div>
          <label className="block text-sm font-medium text-foreground mb-2">Ngày nhận phòng</label>
          <Popover>
            <PopoverTrigger asChild>
              <Button variant="outline" className="w-full justify-start text-left font-normal bg-transparent">
                <CalendarIcon className="mr-2 h-4 w-4" />
                {checkIn ? format(checkIn, "dd/MM/yyyy") : "Chọn ngày"}
              </Button>
            </PopoverTrigger>
            <PopoverContent className="w-auto p-0" align="start">
              <Calendar
                mode="single"
                selected={checkIn || undefined}
                onSelect={(date) => setCheckIn(date || null)}
                disabled={(date) => date < new Date()}
                initialFocus
              />
            </PopoverContent>
          </Popover>
        </div>

        {/* Check-out Date */}
        <div>
          <label className="block text-sm font-medium text-foreground mb-2">Ngày trả phòng</label>
          <Popover>
            <PopoverTrigger asChild>
              <Button variant="outline" className="w-full justify-start text-left font-normal bg-transparent">
                <CalendarIcon className="mr-2 h-4 w-4" />
                {checkOut ? format(checkOut, "dd/MM/yyyy") : "Chọn ngày"}
              </Button>
            </PopoverTrigger>
            <PopoverContent className="w-auto p-0" align="start">
              <Calendar
                mode="single"
                selected={checkOut || undefined}
                onSelect={(date) => setCheckOut(date || null)}
                disabled={(date) => date < (checkIn || new Date())}
                initialFocus
              />
            </PopoverContent>
          </Popover>
        </div>
        {/* Rooms input */}
        <div>
          <label className="block text-sm font-medium text-foreground mb-2">Số phòng</label>
          <div className="relative">
            <Input
              type="number"
              min={1}
              value={rooms}
              onChange={(e: React.ChangeEvent<HTMLInputElement>) => setRooms(Math.max(1, Number.parseInt(e.target.value || "1")))}
              className="pl-3 w-20 sm:w-24 h-9 leading-9 py-0"
            />
          </div>
        </div>

        {/* Guests (label above input) and Search button as separate siblings */}
        <div className="flex flex-col justify-between h-full">
          <div className="flex items-center gap-4 md:gap-10">
            <div className="flex flex-col w-max">
              <label className="block text-sm font-medium text-foreground mb-2 text-center w-full">Số khách</label>
              <div className="flex items-center">
                <Users className="mr-2 h-4 w-4" />
                <Input
                  type="number"
                  min={1}
                  value={guests}
                  onChange={(e: React.ChangeEvent<HTMLInputElement>) => setGuests(Math.max(1, Number.parseInt(e.target.value || "1")))}
                  className="pl-1 w-20 sm:w-24 h-9 leading-9 py-0"
                />
              </div>
            </div>

            <Button onClick={handleSearch} variant="primary" size="md" className="min-w-[140px] md:min-w-[160px] flex items-center justify-center whitespace-nowrap px-4 py-2">
              <Search className="mr-2 h-5 w-5 flex-shrink-0" />
              Tìm kiếm
            </Button>
          </div>
        </div>
      </div>
      {/* for small screens, keep a centered search button */}
      <div className="mt-4 flex justify-center lg:hidden">
        <Button onClick={handleSearch} variant="primary" size="lg" className="w-full px-8 py-2 text-lg font-semibold whitespace-nowrap">
          <Search className="mr-2 h-5 w-5 flex-shrink-0" />
          Tìm kiếm
        </Button>
      </div>
    </Card>
  )
}
