"use client"

export const Label = ({ children, htmlFor, className = "", ...props }: any) => (
  <label htmlFor={htmlFor} className={className} {...props}>
    {children}
  </label>
)

export default Label
