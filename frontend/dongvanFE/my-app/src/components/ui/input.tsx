"use client"

export const Input = ({ className = "", ...props }: any) => (
  <input {...props} className={`border rounded px-3 py-2 ${className}`} />
)

export default Input
