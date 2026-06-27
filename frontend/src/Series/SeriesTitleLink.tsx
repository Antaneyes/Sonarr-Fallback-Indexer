import React from 'react';
import Link, { LinkProps } from 'Components/Link/Link';

export interface SeriesTitleLinkProps extends LinkProps {
  titleSlug: string;
  title: string;
  displayTitle?: string;
}

export default function SeriesTitleLink({
  titleSlug,
  title,
  displayTitle,
  ...linkProps
}: SeriesTitleLinkProps) {
  const link = `/series/${titleSlug}`;

  return (
    <Link to={link} {...linkProps}>
      {displayTitle || title}
    </Link>
  );
}
