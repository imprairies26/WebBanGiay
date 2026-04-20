# Design System Documentation: Kinetic Precision

## 1. Overview & Creative North Star
**Creative North Star: Kinetic Precision**
This design system is built to bridge the gap between high-performance athletics and high-fashion editorial. We are moving away from the "grid-of-boxes" e-commerce template. Instead, we treat the UI as a high-end gallery where the product—the shoe—is the undisputed protagonist. 

By leveraging **Kinetic Precision**, we use intentional asymmetry and bold typographic scales to create a sense of movement. The layout should feel like a high-speed capture: blurred backgrounds, sharp focal points, and a "gravity-defying" layering system that makes products feel light, fast, and premium.

---

## 2. Colors: The High-Octane Palette
The palette utilizes a "Sporty Orange" to drive action against a "Deep Charcoal and Bone" backdrop.

### Surface Hierarchy & Nesting
To achieve a premium feel, we strictly follow **Tonal Layering**. We do not use lines to separate content; we use depth.
- **Base Layer:** Use `surface` (#f8f9fa) for the primary page background.
- **Sectioning:** Use `surface_container_low` (#f3f4f5) to define large content blocks.
- **Interactive Elements:** Use `surface_container_lowest` (#ffffff) for cards or elevated UI elements to make them "pop" against the off-white background.

### The "No-Line" Rule
**Standard 1px borders are strictly prohibited for sectioning.** Boundaries must be defined solely through background color shifts. For example, a product description block (`surface_container_low`) should sit flush against the main `surface` without a stroke.

### The Glass & Gradient Rule
To inject "soul" into the digital experience:
- **Glassmorphism:** Use semi-transparent `surface` colors with a `backdrop-blur` (20px+) for floating navigation bars and modal overlays.
- **Signature Gradients:** Use a subtle linear gradient from `primary` (#a83900) to `primary_container` (#ff5a00) at a 135-degree angle for primary CTA buttons. This prevents the orange from feeling "flat" or "cheap."

---

## 3. Typography: Editorial Authority
We use a high-contrast typographic scale to create a sense of hierarchy and brand "voice."

*   **Headings (Epilogue):** Bold, aggressive, and sans-serif. 
    *   **Display-lg (3.5rem):** Used for hero slogans. Tracking should be tightened (-2%) to feel "locked in."
    *   **Headline-md (1.75rem):** Used for product names and category titles.
*   **UI & Body (Manrope):** Clean, geometric, and highly legible.
    *   **Body-lg (1rem):** For product descriptions. High line-height (1.6) for readability.
    *   **Label-md (0.75rem):** Used for technical specs (e.g., "Weight: 240g"). Always uppercase with increased letter spacing (+5%) for a "technical" look.

---

## 4. Elevation & Depth: The Stacking Principle
We avoid the "pasted-on" look of traditional shadows in favor of ambient, natural light.

- **Ambient Shadows:** When a floating effect is required (e.g., a "Buy Now" sticky bar), use a shadow tinted with `on_surface` (#191c1d).
    *   *Spec:* `offset: 0 20px, blur: 40px, spread: -10px, opacity: 6%`.
- **The "Ghost Border" Fallback:** If a border is required for accessibility on a white-on-white interface, use `outline_variant` (#e4beb1) at **15% opacity**. It should be felt, not seen.
- **Layering Example:** Place a `surface_container_lowest` card on top of a `surface_container_high` section. The contrast in "tone" creates the lift naturally.

---

## 5. Components

### Buttons (The Kinetic Drivers)
- **Primary:** Gradient fill (`primary` to `primary_container`), `9999px` (Full) roundedness. Typography: `title-sm` in `on_primary`. 
- **Secondary:** Transparent background with a `Ghost Border`. On hover, fill with `primary_fixed_dim` at 10% opacity.
- **Tertiary:** Text-only (`primary` color) with a custom "kinetic arrow" icon that slides 4px to the right on hover.

### High-Quality Product Cards
- **Structure:** No borders. Use `surface_container_lowest`.
- **Photography:** The shoe should be masked with no background, allowing it to "overlap" the edges of the card container slightly (using negative margins) to break the box.
- **Interaction:** On hover, the card should scale to 1.02x with a soft ambient shadow appearing underneath.

### Sleek Navigation
- **Style:** Fixed at the top. Use a `surface` fill at 80% opacity with a heavy `backdrop-blur`. 
- **Active State:** A 4px `primary` dot centered beneath the nav item—never an underline.

### Interactive Filters
- **Filter Chips:** Use `secondary_container` with `md` (0.375rem) roundedness. 
- **Active State:** Swap to `primary` background with `on_primary` text. No "checkbox" icons; the color shift is the indicator.

### Input Fields
- **Style:** Minimalist. Only a bottom border using `outline` (#907065) at 30% opacity. 
- **Focus State:** Bottom border transforms into a 2px `primary` line. Label slides up and shrinks to `label-sm`.

---

## 6. Do’s and Don’ts

### Do
- **Use "Aggressive" Whitespace:** Give product photos twice as much room as you think they need.
- **Asymmetric Grids:** Offset images from text blocks to create a dynamic, editorial feel.
- **Product First:** Ensure the shoe photography is high-contrast and professionally lit.

### Don’t
- **Don’t use 100% Black:** Always use `on_surface` (#191c1d) for text to maintain a premium "Charcoal" softness.
- **Don’t use Dividers:** Avoid horizontal rules (`<hr>`). Use a 64px or 80px vertical gap to separate sections.
- **Don’t use Default Shadows:** Never use the browser/Figma default drop shadow. Always tint and diffuse.
- **Don't Over-round:** Keep functional UI (inputs/cards) at `md` (0.375rem) or `lg` (0.5rem) roundedness. Reserve `full` roundedness only for buttons and chips.