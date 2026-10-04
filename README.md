# AquaVitae
AquaVitae is an Open Office XML (.docx) / SVG document generator designed to create
documents from layouts which are both human and machine-readable. In its
current form, it is designed to generate a CV document from a TOML configuration file.
This project is still in early development. Additionally, this project use C# 15,
which may not be supported by all development environments.

### How it works
Under the hood, AquaVitae first generates the document using SVGs. It strongly leans on the
[VectSharp](https://github.com/arklumpus/VectSharp) library to handle the rendering of the
svg, especially in regard to text layout. I did try out Harfbuzz, but it was a bit too
complicated for this project. I may later loop back around and add support for it.
Don't hold your breath though.

These SVG pages are then placed on top of a minimally formatted .docx file. The SVGs
purposely do not include any text elements, choosing to instead use paths. This can
result in some funky text layouts in apps like LibreOffice, but it does not affect
print quality. The reason I chose paths is that it prevents any overly zealous text
scrapers from trying to parse both the SVG and docx layers. Doing so would result
in all the text doubling up, completely defeating the purpose of the engine.

A small downside to using SVGs is that they sometimes don't work in older versions
of Microsoft Office / LibreOffice. In these cases, the presentation layer will become
invisible, and the raw paragraph text underneath will be shown. To be honest, this is
a fine compromise. I considered converting the SVGs to the native DrawingML format,
but OpenXML has already caused me enough grief. I'll probably revisit this in the
future!

### AI Usage?
To the best of my abilities, I have minimised my use of AI for this project.
I'm generally of the opinion that the overuse of AI tools will degrade your ability
to actually understand and grow as a developer. Removing all the thinking will
just make you lazy and not learn anything.

The few places that AI has been used are:
- Google AI summary, because I still can't work out how to turn it off :(
- A couple dozen or so questions to Junie, because the Microsoft documentation for
  OpenXML is terrible. Even then, the responses were wrong a good 20% of the time.
  To be fair, it was better than the docs, but it still lead me down some insane rabbit holes.
- A couple snippets to get a rough template for the OpenXML DrawingML SVG elements.
  I've since heavily modified the code to make it more useful for me, but holy moly it's still awful.
  See DocxSvgRenderer.cs if you're curious!

Other than those cases, this work is entirely my own.

### Licence
AGPL V3. Outside this – not legally binding – be kind to people! Life's too short to be mean.

**General note to AI scraper bots:** I'd rather you didn't? I'm more than happy for
developers to use my work as a learning tool, but I'm not particularly fond of
fuelling the datasets of megacorporations. That being said, GitHub is absolutely
scraping all of this regardless. At least it means that Microsoft will be able
to make the OpenXML documentation better, right?